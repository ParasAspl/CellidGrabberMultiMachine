import cellid.grabber.RowNumberTable;
import cellid.utils.DisconnectException;
import cellid.utils.TwoWaySerialComm2;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClientBuilder;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.util.EntityUtils;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.json.JSONObject;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableModel;
import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;

import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.*;
import java.net.http.HttpClient;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.security.Key;
import java.security.NoSuchAlgorithmException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Base64;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.List;

public class CellIdGrabberMain {

	protected static TimerThread timerThread;

	public static TwoWaySerialComm2 mySerial;

	public static boolean connectFlag = false;
	public static boolean routeRunning = false;
	public static boolean isRouteScan = false;
	public static boolean scanRunning = false;

	static Color ColorTitle = new Color(117, 117, 117);
	static Color ColorPrimaryBackground = new Color(224, 224, 224);
	static Color tableColor = new Color(224, 242, 241);

	static Map<String, String[]> regionCircleMap = new HashMap<>();
	static Map<String, List<String>> regionMCCMap = new HashMap<>();

	private static final String ALGORITHM = "AES";
	private static final byte[] keyValue = 
	    new byte[] { 'w', 'i', 'l', 'd','h','u','l','k','w', 'i', 'l', 'd','h','u','l','k' };

	 public static String encrypt(String valueToEnc) throws Exception {
	    Key key = generateKey();
	    Cipher c = Cipher.getInstance(ALGORITHM);
	    c.init(Cipher.ENCRYPT_MODE, key);
	    byte[] encValue = c.doFinal(valueToEnc.getBytes());
	    String encryptedValue = Base64.getEncoder().encodeToString(encValue);
	    return encryptedValue;
	}

	public static String decrypt(String encryptedValue) throws Exception {
	    Key key = generateKey();
	    Cipher c = Cipher.getInstance(ALGORITHM);
	    c.init(Cipher.DECRYPT_MODE, key);
	    byte[] decordedValue = Base64.getDecoder().decode(encryptedValue);
	    byte[] decValue = c.doFinal(decordedValue);
	    String decryptedValue = new String(decValue);
	    return decryptedValue;
	}

	private static Key generateKey() throws Exception {
	    Key key = new SecretKeySpec(keyValue, ALGORITHM);
	    return key;
	}
	
	private static RequestConfig requestConfig = RequestConfig.custom().build();
	
	private static InputStream getFileFromResourceAsStream(String fileName) {

		// The class loader that loaded the class
		CellIdGrabberMain obj = new CellIdGrabberMain();
		ClassLoader classLoader = obj.getClass().getClassLoader();
		InputStream inputStream = classLoader.getResourceAsStream(fileName);

		// the stream holding the file content
		if (inputStream == null) {
			throw new IllegalArgumentException("file not found! " + fileName);
		} else {
			return inputStream;
		}

	}
	public static HttpResponse postWithFormData(String url, List<NameValuePair> params) throws IOException {
        // building http client
        CloseableHttpClient httpClient = HttpClientBuilder.create().setDefaultRequestConfig(requestConfig).build();
        HttpPost request = new HttpPost(url);

        // adding the form data
        request.setEntity(new UrlEncodedFormEntity(params));
        return httpClient.execute(request);
    }

	private static String validateKey(String key) throws Exception {
		List<NameValuePair> urlParameters = new ArrayList<>();

		// add any number of form data
		BasicNameValuePair bp = new BasicNameValuePair("key", key);
		urlParameters.add(bp);
		String result = null;
		try {
			HttpResponse response = postWithFormData("https://msg.ccas.in/api/cellId/productKey", urlParameters);
			HttpEntity entity = response.getEntity();
			// String of the response
			String responseString = EntityUtils.toString(entity);
			// JSON of the response (use this only if the response is a JSON)
			System.out.println("responseString=" + responseString);
			JSONObject responseObject = new JSONObject(responseString);
			result = (String) responseObject.get("error");
			System.out.println("result=" + result);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return result;
	}

	public static void infoBox(String infoMessage, String titleBar) {
		JOptionPane.showMessageDialog(null, infoMessage, "InfoBox: " + titleBar, JOptionPane.INFORMATION_MESSAGE);
	}

	public static void main(String[] args) throws Exception {

		
		/*
		 * Files.deleteIfExists(Paths.get("./out.txt")); PrintStream fileOut = new
		 * PrintStream("./out.txt"); System.setOut(fileOut); System.setErr(fileOut);
		 */
		String motherboard = null;
		try
	    {
	        String result = "";
	        Process p = Runtime.getRuntime().exec("wmic baseboard get serialnumber");
	        BufferedReader input
	                = new BufferedReader(new InputStreamReader(p.getInputStream()));
	        String line;
	        while ((line = input.readLine()) != null)
	        {
	        	//System.out.println("line=" + line);
	            result += line;
	        }
	        if (result.equalsIgnoreCase(" ")) {
	            System.out.println("Result is empty");
	        } else
	        {
	            motherboard=result.replaceAll(" ", "");
	        }
	        input.close();
	    } catch (IOException ex)
	    {
	        ex.printStackTrace();
	    }
		String key = null;
		if(!Files.exists(Paths.get("./system.dat"))) {
			infoBox("Key not present", "Devie Key");
			key = JOptionPane.showInputDialog("Please enter key");
			String status = validateKey(key);
			if(status != null && status.equalsIgnoreCase("Licence key has been verified.")) {
				infoBox("License key has been verified", "Devie Key");
				File f = new File("./system.dat");
				String passphraseEncrypt = encrypt(key+ ";" +motherboard);
				FileWriter fw = new FileWriter(f);
				fw.write(passphraseEncrypt);
				fw.close();
			} else {
				infoBox("License key not verified", "Devie Key");
				System.exit(0);
			}
		} else {
			File f = new File("./system.dat");
			FileReader fr = new FileReader(f);
			BufferedReader br = new BufferedReader(fr);
			String passphraseEncrypt = br.readLine();
			String passphrase = decrypt(passphraseEncrypt);
			String str[] = passphrase.split(";");
			String keyStr = str[0];
			String motherboardStr = str[1];
			if(!motherboardStr.equalsIgnoreCase(motherboard)) {
				infoBox("License key is already in use.", "Devie Key");
				System.exit(0);
			}
			br.close();
			fr.close();
			System.out.println("keyStr=" + keyStr);
			String status = validateKey(keyStr);
			if(status != null && status.equalsIgnoreCase("Licence key has been deactivated, please contact to admin.")) {
				infoBox("License key has been deactivated, please contact to admin.", "Devie Key");
				Files.deleteIfExists(Paths.get("./system.dat"));
				System.exit(0);
			}
			
		}

		
		String ppp = "COM72";

		try {
			Runtime rt = Runtime.getRuntime();
			String cmd = "cmd /c wmic path win32_pnpentity get caption /format:table  <NUL";

			Process pr = rt.exec(cmd);

			InputStreamReader tempInputStream = new InputStreamReader(pr.getInputStream());
			BufferedReader tempReader = new BufferedReader(tempInputStream);
			String line = null;
			while ((line = tempReader.readLine()) != null) {
				System.out.println(line);

				if (line.contains("AT Port")) {
					ppp = line.substring(line.lastIndexOf('(') + 1, line.lastIndexOf(')'));
				}

			}

			System.out.println("port:" + ppp);

			tempInputStream.close();
			pr.getOutputStream().close();
			pr.getInputStream().close();

		} catch (Exception e) {

		}

		final String port = ppp;

		String[] header = { "Date Time", "Circle", "Operator Name", "MCC", "MNC", "LAC", "ECI", "CellId", "CGI",
				"(A/E/U)RFCN", "ENB", "Network Type", "BSIC/PSC/PCI", "dBm", "Net. Strength  " };
		DefaultTableModel dtm = new DefaultTableModel(null, header) {

			/*
			 * @Override public Class<?> getColumnClass(int col) { return getValueAt(0,
			 * col).getClass(); }
			 */
		};

		JTable table = new JTable(dtm);
		InputStream regionCircleFile = getFileFromResourceAsStream("mcc-mnc.txt");
		try {
			BufferedReader br = new BufferedReader(new InputStreamReader(regionCircleFile));
			String line;
			while ((line = br.readLine()) != null) {
				String[] splits = line.split("\\|\\|");
				if (splits.length >= 4) {
					// System.out.println("line: " + line);
					String entry[] = new String[2];
					entry[0] = splits[2].trim();
					entry[1] = splits[3].trim();
					// System.out.println(splits[0].trim()+"-"+ splits[1].trim());
					regionCircleMap.put(splits[0].trim() + "-" + splits[1].trim(), entry);

					List<String> mccMNC = regionMCCMap.get(splits[3].trim());
					if (mccMNC == null) {
						mccMNC = new ArrayList<>();
					}
					mccMNC.add(splits[0] + splits[1]);
					regionMCCMap.put(splits[3].trim(), mccMNC);
				}
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}

		Set<String> keySet = regionCircleMap.keySet();

		/*
		 * for(String str: keySet) { System.out.println("Key: " + str); }
		 * 
		 */
		mySerial = new TwoWaySerialComm2(dtm, regionCircleMap);

		Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
		System.out.println(screenSize);
		int screenStartX = 0;
		int screenStartY = 0;
		int width = screenSize.width;
		int height = screenSize.height - screenStartY - 40;
//        UIManager.put("InternalFrame.activeTitleBackground", new ColorUIResource(Color.black ));
		JFrame frame = new JFrame("LIS - Cell ID Grabber");
		frame.setLayout(null);
		frame.setResizable(false);
		frame.setBackground(ColorPrimaryBackground);
//        frame.setUndecorated(true);
		frame.setBounds(screenStartX, screenStartY, width, height);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

		System.out.println(frame.getSize());
		System.out.println(frame.getBounds());

		// head panel start

		JPanel panelHead = new JPanel();
		int panelHeadX = screenStartX;
		int panelHeadY = screenStartY;
		int panelHeadWidth = width;
		int panelHeadHeight = (int) (height * 0.20);
		panelHead.setBackground(ColorPrimaryBackground);
		panelHead.setBounds(panelHeadX, panelHeadY, panelHeadWidth, panelHeadHeight);
		System.out.println(panelHead.getBounds());
		panelHead.setLayout(null);

		// Status Bar start

		int statusBarX = panelHeadX + 5;
		int statusBarY = panelHeadY + 5;
		int statusBarWidth = width - 10;
		int statusBarHeight = (int) (panelHeadHeight * 0.25) - 5;
		JPanel statusBar = new JPanel();
		statusBar.setBounds(statusBarX, statusBarY, statusBarWidth, statusBarHeight);
		statusBar.setBackground(ColorTitle);
		panelHead.add(statusBar);
		statusBar.setLayout(new GridLayout());

		final JLabel dateLabel = new JLabel();
		dateLabel.setForeground(Color.WHITE);
		dateLabel.setHorizontalAlignment(SwingConstants.CENTER);
		statusBar.add(dateLabel, BorderLayout.EAST);

		JLabel statusLabel = new JLabel("Application state : Running", SwingConstants.CENTER);
		statusLabel.setForeground(Color.WHITE);
		statusBar.add(statusLabel, BorderLayout.CENTER);

		final JLabel timeLabel = new JLabel();
		timeLabel.setForeground(Color.WHITE);
		timeLabel.setHorizontalAlignment(SwingConstants.CENTER);
		statusBar.add(timeLabel, BorderLayout.WEST);

		timerThread = new TimerThread(dateLabel, timeLabel);
		timerThread.start();

		// Status bar end

		// info panel starts

		int infoBarX = panelHeadX + 5;
		int infoBarY = (int) (panelHeadHeight * 0.25) + 5;
		int infoBarWidth = width - 10;
		int infoBarHeight = (int) (panelHeadHeight * 0.25) - 10;

		JPanel infoBar = new JPanel();
		infoBar.setBounds(infoBarX, infoBarY, infoBarWidth, infoBarHeight);
		infoBar.setBackground(ColorPrimaryBackground);
		panelHead.add(infoBar);
		infoBar.setLayout(new GridLayout());

		final JLabel gpsLabel = new JLabel("© Avenging Security");
		gpsLabel.setForeground(Color.BLACK);
		gpsLabel.setHorizontalAlignment(SwingConstants.CENTER);
		infoBar.add(gpsLabel, BorderLayout.EAST);

		String spotModes[] = { "ALL", "2G+4G", "2G", "2G+3G", "4G" };
		String routeModes[] = { "2G", "3G", "4G" };
		JComboBox modesList = new JComboBox(spotModes);

		String route[] = { "Spot", "Route" };
		JComboBox routeList = new JComboBox(route);
		infoBar.add(routeList, BorderLayout.CENTER);
//        modesList.removeAllItems();

		infoBar.add(modesList, BorderLayout.CENTER);
//        modesList.removeAllItems();

		final JLabel infoLabel = new JLabel("Contact details: 9461101915");
		infoLabel.setForeground(Color.BLACK);
		infoLabel.setHorizontalAlignment(SwingConstants.CENTER);
		infoBar.add(infoLabel, BorderLayout.WEST);

		// // info panel ends

		// command bar start
		int commandBarX = panelHeadX + 5;
		int commandBarY = (int) (panelHeadHeight * 0.5) + 5;
		int commandBarWidth = width - 10;
		int commandBarHeight = (int) (panelHeadHeight * 0.5) - 5;
		JPanel commandBar = new JPanel();
		commandBar.setBounds(commandBarX, commandBarY, commandBarWidth, commandBarHeight);
		commandBar.setBackground(ColorTitle);
		panelHead.add(commandBar);
		commandBar.setLayout(new BorderLayout());

		JPanel progressPanel = new JPanel();
		progressPanel.setBackground(ColorPrimaryBackground);
		progressPanel.setLayout(new GridLayout());
		progressPanel.setBounds(commandBarX + 20, commandBarY + (int) (panelHeadHeight * 0.25), commandBarWidth - 40,
				(int) (commandBarHeight * 0.5));
		progressPanel.setBackground(ColorPrimaryBackground);
		JProgressBar progress = new JProgressBar(SwingConstants.HORIZONTAL, 0, 100);
		progressPanel.add(progress);
		progress.setForeground(Color.GREEN);
		progress.setValue(0);
		commandBar.add(progressPanel, BorderLayout.SOUTH);

		JPanel actionPanel = new JPanel();
		actionPanel.setLayout(new GridLayout());
		actionPanel.setBounds(commandBarX, commandBarY + 5, commandBarWidth, (int) (commandBarHeight * 0.25));
		actionPanel.setBackground(ColorPrimaryBackground);
		commandBar.add(actionPanel, BorderLayout.CENTER);

		JButton connectButton = new JButton("Connect");
		connectButton.setBackground(ColorPrimaryBackground);
//        connectButton.setContentAreaFilled(false);
		connectButton.setOpaque(true);
		connectButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside button listener");

						if (connectFlag) {
							System.out.println("Device already connected");
							infoBox("Device already connected", "Device Status");
							return Integer.valueOf(2);
						}
						String ret;
//                        String port1 = System.getProperty("commPort");
//                        String speed = System.getProperty("portSpeed");
						String speed = "115200";
						int sp1 = Integer.parseInt(speed);
						try {
							System.out.println("Inside button listener 1 an port: " + port);
							ret = mySerial.connect(port, sp1);
							System.out.println("ret: " + ret);
							try {
								Thread.sleep(3000L);
							} catch (InterruptedException localInterruptedException) {
								statusLabel.setText("Device Status : Connection Failed");
								infoBox("Connection Failed", "Device Status");
							}

							if (ret.equals("connect_sucess")) {
//                                    TwoWaySerialComm2.send_to_parser = 1;
								statusLabel.setText("Device Status : Connected");
								connectFlag = true;
								infoBox("Connected", "Device Status");
							} else if (ret.equals("connect_port_in_use")) {
								statusLabel.setText("Device Status : Connection Failed");
								infoBox("Connection Failed", "Device Status");
							} else {
								statusLabel.setText("Device Status : Connection Failed");
								infoBox("Connection Failed", "Device Status");
							}
						} catch (Exception et) {
							System.out.println("Inside button listener3");
							et.printStackTrace();
							statusLabel.setText("Device Status : Connection Failed");
							infoBox("Connection Failed", "Device Status");
						}
						System.out.println("Inside button listener4");
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton disconnectButton = new JButton("Disconnect");
		disconnectButton.setBackground(ColorPrimaryBackground);
		disconnectButton.setOpaque(true);
		disconnectButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside Disconnect button listener");
						String ret;
						try {
							System.out.println("Inside button listener 1");
							mySerial.disconnect();
						} catch (Exception et) {
							System.out.println("Inside button listener3");
							et.printStackTrace();
						} finally {
							connectFlag = false;
							scanRunning = false;
							mySerial.scanRunning = false;
							statusLabel.setText("Device Status : Disconnected");
							infoBox("Disconnected", "Device Status");
						}
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton scanButton = new JButton("Scan");
		scanButton.setBackground(ColorPrimaryBackground);
		scanButton.setOpaque(true);
		scanButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside Disconnect button listener");
						String ret;
						try {
							DateFormat df = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
							Date dateobj = new Date();
							mySerial.date = df.format(dateobj);
							if (mySerial.serialPort == null) {
								System.out.println("IDevice is not connected");
								infoBox("Device is not connected", "Scan Status");
							} else {
								statusLabel.setText("Device Status : Scanning");
								progress.setValue(10);
								mySerial.scanRunning = true;
								scanRunning = true;
								String mode = (String) modesList.getSelectedItem();
								System.out.println("mode: " + mode);
								if (mode == null) {
									mode = "ALL";
								}
								if (mode.equalsIgnoreCase("ALL") || mode.equalsIgnoreCase("2G")
										|| mode.equalsIgnoreCase("2G+3G")) {
									scan2GNetwork(progress);
								}
								if (mode.equalsIgnoreCase("ALL") || mode.equalsIgnoreCase("4G")) {
									scan4GNetwork(progress);
								}
								if (mode.equalsIgnoreCase("ALL") || mode.equalsIgnoreCase("3G")
										|| mode.equalsIgnoreCase("2G+3G")) {
									scan3GNetwork(progress);
								}
								if (mode.equalsIgnoreCase("2G+4G")) {
									scan2GNetwork(progress);
									scan4GNetwork(progress);
								}
								progress.setValue(100);
							}
						} catch (Exception et) {
							System.out.println("Inside button listener3");
							infoBox("Scan stopped", "Scan Status");
							et.printStackTrace();
						} finally {
							if (connectFlag) {
								infoBox("Scan completed", "Scan Status");
								statusLabel.setText("Device Status : Connected");
							}
							scanRunning = false;
							Thread.sleep(5000);
							mySerial.scanRunning = false;
							progress.setValue(100);
						}
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton startButton = new JButton("Start");
		startButton.setBackground(ColorPrimaryBackground);
		startButton.setOpaque(true);
		startButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside Disconnect button listener");
						String ret;
						try {
							DateFormat df = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
							Date dateobj = new Date();
							mySerial.date = df.format(dateobj);
							if (mySerial.serialPort == null) {
								System.out.println("IDevice is not connected");
								infoBox("Device is not connected", "Scan Status");
							} else {
								statusLabel.setText("Device Status : Scanning");
								progress.setValue(10);
								mySerial.scanRunning = true;
								scanRunning = true;
								routeRunning = true;
								String mode = (String) modesList.getSelectedItem();
								System.out.println("mode: " + mode);
								if (mode == null) {
									mode = "4G";
								}
								if (mode.equalsIgnoreCase("2G")) {
									scan2GNetworkRoute(progress);
								}
								if (mode.equalsIgnoreCase("4G")) {
									scan4GNetworkRoute(progress);
								}
								if (mode.equalsIgnoreCase("3G")) {
									scan3GNetworkRoute(progress);
								}
								progress.setValue(100);
							}
						} catch (Exception et) {
							System.out.println("Inside button listener3");
							infoBox("Rute scan stopped", "Scan Status");
							et.printStackTrace();
						} finally {
							if (connectFlag) {
								infoBox("Route scan completed", "Scan Status");
								statusLabel.setText("Device Status : Connected");
							}
							scanRunning = false;
							routeRunning = false;
							mySerial.scanRunning = false;
							progress.setValue(100);
						}
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton stopButton = new JButton("Stop");
		stopButton.setBackground(ColorPrimaryBackground);
		stopButton.setOpaque(true);
		stopButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside Stop button listener");
						String ret;
						try {
							DateFormat df = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
							Date dateobj = new Date();
							mySerial.date = df.format(dateobj);
							if (mySerial.serialPort == null) {
								System.out.println("IDevice is not connected");
								infoBox("Device is not connected", "Scan Status");
							} else {
								statusLabel.setText("Device Status : Scanning");
								progress.setValue(10);
								mySerial.scanRunning = false;
								scanRunning = false;
								routeRunning = false;
								progress.setValue(100);
							}
						} catch (Exception et) {
							System.out.println("Inside button listener3");
							infoBox("Ruote stopped", "Ruote Status");
							et.printStackTrace();
						} finally {
							if (connectFlag) {
								infoBox("Scan stopped", "Scan Status");
								statusLabel.setText("Device Status : Connected");
							}
							scanRunning = false;
							routeRunning = false;
							mySerial.scanRunning = false;
							progress.setValue(100);
						}
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton clearLogsButton = new JButton("Clear Logs");
		clearLogsButton.setBackground(ColorPrimaryBackground);
		clearLogsButton.setOpaque(true);
		clearLogsButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				new SwingWorker() {
					protected Integer doInBackground() throws Exception {
						System.out.println("Inside Clear Logs button listener");
						String ret;
						if (scanRunning) {
							infoBox("Run clear log after scan is completed", "Clear Log");
							return Integer.valueOf(1);
						}
						try {
							System.out.println("Inside try button clearLogsButton");
							for (int i = dtm.getRowCount() - 1; i >= 0; i--) {
								dtm.removeRow(i);
							}
						} catch (Exception et) {
							System.out.println("Inside button clearLogsButton");
							et.printStackTrace();
						} finally {
							mySerial.resetSetValues();
						}
						System.out.println("Clear Logs completed");
						return Integer.valueOf(1);
					}
				}.execute();
			}
		});

		JButton saveLogsButton = new JButton("Save Logs");
		saveLogsButton.setBackground(ColorPrimaryBackground);
		saveLogsButton.setOpaque(true);
		saveLogsButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent actionEvent) {
				System.out.println("Inside Clear Logs button listener");
				String ret;
				try {
					JFileChooser fileChooser = new JFileChooser();
					int retval = fileChooser.showSaveDialog(saveLogsButton);

					if (retval == JFileChooser.APPROVE_OPTION) {
						File file = fileChooser.getSelectedFile();
						if (file != null) {
							if (!file.getName().toLowerCase().endsWith(".xlsx")) {
								file = new File(file.getParentFile(), file.getName() + ".xlsx");
							}
							try {
								// new WorkbookFactory();
								Workbook wb = new XSSFWorkbook();
								Sheet sheet = wb.createSheet("All"); // WorkSheet
								Row row = sheet.createRow(2); // Row created at line 3
								TableModel model = table.getModel(); // Table model
								System.out.println("rows: " + model.getRowCount());

								Row headerRow = sheet.createRow(0); // Create row at line 0
								for (int headings = 0; headings < model.getColumnCount(); headings++) { // For each
																										// column
									headerRow.createCell(headings).setCellValue(model.getColumnName(headings));// Write
																												// column
																												// name
								}

								for (int rows = 0; rows < model.getRowCount(); rows++) { // For each table row
									for (int cols = 0; cols < table.getColumnCount(); cols++) {
										// For each table column
										if (model.getValueAt(rows, cols) != null) {
											row.createCell(cols).setCellValue(model.getValueAt(rows, cols).toString()); // Write
																														// value
										} else {
											row.createCell(cols).setCellValue("");
										}
									}

									// Set the row to the next one in the sequence
									row = sheet.createRow((rows + 3));
								}
								FileOutputStream fos = new FileOutputStream(file);
								wb.write(fos);
								fos.close();
								System.out.println(file.getName() + " written successfully");
//                                            wb.write(new FileOutputStream(file));//Save the file
							} catch (UnsupportedEncodingException e) {
								e.printStackTrace();

							} catch (FileNotFoundException e) {
								e.printStackTrace();
								System.out.println("not found");
							} catch (IOException e) {
								e.printStackTrace();
							}
						}
					}

				} catch (Exception e) {
					e.printStackTrace();
					System.out.println("shit");
				}
				System.out.println("Clear Logs completed");

			}

		});

		routeList.addActionListener(new ActionListener() {

			public void actionPerformed(ActionEvent e) {
				String routeValue = (String) routeList.getSelectedItem();
				if (routeValue.equalsIgnoreCase("Route")) {
					isRouteScan = true;
					modesList.removeAllItems();
					for (String str : routeModes) {
						modesList.addItem(str);
					}

					actionPanel.removeAll();
					actionPanel.add(connectButton);
					actionPanel.add(disconnectButton);
					actionPanel.add(startButton);
					actionPanel.add(stopButton);
					actionPanel.add(clearLogsButton);
					actionPanel.add(saveLogsButton);
				} else {
					isRouteScan = false;
					modesList.removeAllItems();
					for (String str : spotModes) {
						modesList.addItem(str);
					}
					actionPanel.removeAll();
					actionPanel.add(connectButton);
					actionPanel.add(disconnectButton);
					actionPanel.add(scanButton);
					actionPanel.add(clearLogsButton);
					actionPanel.add(saveLogsButton);
				}

			}
		});

		actionPanel.add(connectButton);
		actionPanel.add(disconnectButton);
		actionPanel.add(scanButton);
		actionPanel.add(clearLogsButton);
		actionPanel.add(saveLogsButton);
		// command bar end

		// head panel end

		// body starts

		JPanel panelBody = new JPanel();
		int panelBodyX = screenStartX;
		int panelBodyY = panelHeadY + panelHeadHeight;
		int panelBodyWidth = width;
		int panelBodyHeight = (int) (height * 0.80);
		panelBody.setBounds(panelBodyX, panelBodyY, panelBodyWidth, panelBodyHeight);
		System.out.println(panelBody.getBounds());
		panelBody.setBackground(ColorTitle);
		panelBody.setLayout(new FlowLayout());

		table.setBackground(tableColor);
		String[] row1 = { "Date Time", "Circle", "Operator Name", "MCC", "MNC", "LAC", "ECI", "CellId", "CGI", "RFCN",
				"ENB", "Network Strength" };

		/*
		 * for(int i = 0 ; i < 50 ; i++) { dtm.addRow(row1); }
		 */

		JTable rowTable = new RowNumberTable(table);
		JScrollPane scrollPane = new JScrollPane(table);
		scrollPane.setVerticalScrollBarPolicy(JScrollPane.VERTICAL_SCROLLBAR_ALWAYS);
		scrollPane.setBounds(panelBodyX, panelBodyY, panelBodyWidth, panelBodyHeight);
		scrollPane.setPreferredSize(new Dimension(panelBodyWidth - 30, panelBodyHeight - 50));
		scrollPane.setBackground(tableColor);
		scrollPane.getVerticalScrollBar().setUnitIncrement(10);
		panelBody.add(scrollPane);

		// body ends

		frame.add(panelHead);
		frame.add(panelBody);
//        frame.pack();
		frame.setVisible(true);
	}

	public static void scan2GNetwork(JProgressBar progress) throws DisconnectException {

		TwoWaySerialComm2.comandCount = 0;
		mySerial.networkType = "2G";
		String retreb = mySerial.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F" + "\r\n",
				1);
		mySerial.waitForOutput();
		try {
			Thread.sleep(5000L);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		String ret = mySerial.ser_write("AT+CNMP=13\r\n", 1);
		mySerial.waitForOutput();

		ret = mySerial.ser_write("AT+CMSSN\r\n", 1);
		mySerial.waitForOutput();

		ret = mySerial.ser_write("AT+CSURV\r\n", 1);
		mySerial.waitForOutput(120);

		/*
		 * String bandS[] = { "0x0000000000000080", "0x0000000000000100",
		 * "0x0000000000000100" }; int arfcn[][] = { { 0, 124 }, { 512, 885 }, { 975,
		 * 1023 } }; for (int i = 0; i < bandS.length; i++) { if (connectFlag == false)
		 * { // infoBox("Device is not connected. Scan will stop", "Scan Status");
		 * return; } // String retb = mySerial.ser_write("AT+CNBP=" + bandS[i] + "\r\n",
		 * 1); // mySerial.waitForOutput(); progress.setValue(progress.getValue() + 10);
		 * TwoWaySerialComm2.comandCount = 0; for (int j = arfcn[i][0]; j < arfcn[i][1];
		 * j += 5) { if (connectFlag == false) { //
		 * infoBox("Device is not connected. Scan will stop", "Scan Status"); return; }
		 * String reta = mySerial.ser_write("AT+CNSVS=" + j + "," + (j + 6) + "\r\n",
		 * 1); mySerial.waitForOutput(); reta = mySerial.ser_write("AT+CCINFO\r\n", 1);
		 * mySerial.waitForOutput(); } }
		 */

		// 2g new method
		if (mySerial.region.size() > 0) {
			System.out.println("Starting 2g");
			List<String> mccMncList = new ArrayList<>();
			for(String regi : mySerial.region) {
				mccMncList.addAll(regionMCCMap.get(regi));
			}
			for (String mccMnc : mccMncList) {
				String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
				mySerial.waitForOutput();
				ret = mySerial.ser_write("AT+CSURV\r\n", 1);
				mySerial.waitForOutput(120);
				ret = mySerial.ser_write("AT+CSURV\r\n", 1);
				mySerial.waitForOutput(120);
			}
			ret = mySerial.ser_write("AT+CMSSN\r\n", 1);
			mySerial.waitForOutput(20);
		} else {
			System.out.println("no region found in 2g");
		}
		mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
		progress.setValue(progress.getValue() + 10);
	}

	public static void scan2GNetworkRoute(JProgressBar progress) throws DisconnectException {

		TwoWaySerialComm2.comandCount = 0;
		mySerial.networkType = "2G";
		String retreb = mySerial.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F" + "\r\n",
				1);
		mySerial.waitForOutput();
		try {
			Thread.sleep(5000L);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		String ret = mySerial.ser_write("AT+CNMP=13\r\n", 1);
		mySerial.waitForOutput();

		ret = mySerial.ser_write("AT+CMSSN\r\n", 1);
		mySerial.waitForOutput();

		ret = mySerial.ser_write("AT+CSURV\r\n", 1);
		mySerial.waitForOutput(20);

		ret = mySerial.ser_write("AT+CSURV\r\n", 1);
		mySerial.waitForOutput(20);

		/*
		 * String bandS[] = { "0x0000000000000080", "0x0000000000000100",
		 * "0x0000000000000100" }; int arfcn[][] = { { 0, 124 }, { 512, 885 }, { 975,
		 * 1023 } }; for (int i = 0; i < bandS.length; i++) { if (connectFlag == false)
		 * { // infoBox("Device is not connected. Scan will stop", "Scan Status");
		 * return; } // String retb = mySerial.ser_write("AT+CNBP=" + bandS[i] + "\r\n",
		 * 1); // mySerial.waitForOutput(); progress.setValue(progress.getValue() + 10);
		 * TwoWaySerialComm2.comandCount = 0; for (int j = arfcn[i][0]; j < arfcn[i][1];
		 * j += 5) { if (connectFlag == false) { //
		 * infoBox("Device is not connected. Scan will stop", "Scan Status"); return; }
		 * String reta = mySerial.ser_write("AT+CNSVS=" + j + "," + (j + 6) + "\r\n",
		 * 1); mySerial.waitForOutput(); reta = mySerial.ser_write("AT+CCINFO\r\n", 1);
		 * mySerial.waitForOutput(); } }
		 */

		// 2g new method
		while (routeRunning) {
			if (mySerial.region.size() > 0) {
				List<String> mccMncList = new ArrayList<>();
				for(String regi : mySerial.region) {
					mccMncList.addAll(regionMCCMap.get(regi));
				}
				for (String mccMnc : mccMncList) {
					if (connectFlag == false) {
						return;
					}
					String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
					mySerial.waitForOutput();
		
					ret = mySerial.ser_write("AT+CSURV\r\n", 1);
					mySerial.waitForOutput(20);
				}
			}
		}
		progress.setValue(progress.getValue() + 10);
		mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
		try {
			Thread.sleep(20000L); // apply this wait for network busy error
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
	}

	public static void scan3GNetwork(JProgressBar progress) throws DisconnectException {

		TwoWaySerialComm2.comandCount = 0;
		mySerial.networkType = "3G";
		String retreb = mySerial.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F\r\n", 1);
		mySerial.waitForOutput();

		String ret = mySerial.ser_write("AT+CNMP=14\r\n", 1);
		mySerial.waitForOutput();
		try {
			Thread.sleep(10000L); // apply this wait for network busy error
		} catch (InterruptedException e) {
			e.printStackTrace();
		}

		/*
		 * String bandS[] = { "0x0000000000000100", "0x0000000000400000" }; int
		 * arfcn[][] = { { 2937, 3088 }, { 10562, 10838 } }; for (int i = 0; i <
		 * bandS.length; i++) { if (connectFlag == false) { //
		 * infoBox("Device is not connected. Scan will stop", "Scan Status"); return; }
		 * // String retb = mySerial.ser_write("AT+CNBP=" + bandS[i] + "\r\n", 1); //
		 * mySerial.waitForOutput(); TwoWaySerialComm2.comandCount = 0; for (int j =
		 * arfcn[i][0]; j < arfcn[i][1]; j++) { if (connectFlag == false) { //
		 * infoBox("Device is not connected. Scan will stop", "Scan Status"); return; }
		 * String reta = mySerial.ser_write("AT+CLUARFCN=" + j + "\r\n", 1);
		 * mySerial.waitForOutput(); String retcc =
		 * mySerial.ser_write("AT+CSNINFO?\r\n", 1); mySerial.waitForOutput(); String
		 * rete = mySerial.ser_write("AT+CCINFO\r\n", 1); mySerial.waitForOutput(); }
		 * progress.setValue(progress.getValue() + 10); }
		 */

		String retaa = mySerial.ser_write("AT+CLUARFCN\r\n", 1);
		mySerial.waitForOutput();

		if (mySerial.region.size() > 0) {

			List<String> mccMncList = new ArrayList<>();
			for(String regi : mySerial.region) {
				mccMncList.addAll(regionMCCMap.get(regi));
			}
			for(int i = 0 ; i < 6; i++) {
				mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
				mySerial.waitForOutput();
				for (String mccMnc : mccMncList) {
					if (connectFlag == false) {
						return;
					}
					String retrr = mySerial
							.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F" + "\r\n", 1);
					mySerial.waitForOutput();
					try {
						Thread.sleep(5000L);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
					String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
					mySerial.waitForOutput();
					String retb = mySerial.ser_write("AT+CLUARFCN\r\n", 1);
					mySerial.waitForOutput();
					String retc = mySerial.ser_write("AT+CLUCELL\r\n", 1);
					mySerial.waitForOutput();
					String retd = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
					mySerial.waitForOutput();
					String rete = mySerial.ser_write("AT+CCINFO\r\n", 1);
					mySerial.waitForOutput();
				}
				String reta = mySerial.ser_write("AT+CMSSN\r\n", 1);
				mySerial.waitForOutput();
			}
		}

	}

	public static void scan3GNetworkRoute(JProgressBar progress) throws DisconnectException {

		TwoWaySerialComm2.comandCount = 0;
		mySerial.networkType = "3G";
		String retreb = mySerial.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F\r\n", 1);
		mySerial.waitForOutput();

		String ret = mySerial.ser_write("AT+CNMP=14\r\n", 1);
		mySerial.waitForOutput();
		try {
			Thread.sleep(10000L); // apply this wait for network busy error
		} catch (InterruptedException e) {
			e.printStackTrace();
		}

		String bandS[] = { "0x0000000000000100", "0x0000000000400000" };
		int arfcn[][] = { { 2937, 3088 }, { 10562, 10838 } };
		while (routeRunning) {
			if (mySerial.region.size() > 0) {

				List<String> mccMncList = new ArrayList<>();
				for(String regi : mySerial.region) {
					mccMncList.addAll(regionMCCMap.get(regi));
				}	
				
				mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
				mySerial.waitForOutput();
				for (String mccMnc : mccMncList) {

					if (connectFlag == false) {
//						infoBox("Device is not connected. Scan will stop", "Scan Status");
						return;
					}

					String retrr = mySerial
							.ser_write("AT+CNBP=0xFFFFFFFF7FFFFFFF,0x000007FF03DF3FFF,0x000000000000003F" + "\r\n", 1);
					mySerial.waitForOutput();
					try {
						Thread.sleep(5000L);
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
					String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
					mySerial.waitForOutput();
					String retb = mySerial.ser_write("AT+CLUARFCN\r\n", 1);
					mySerial.waitForOutput();
					String retc = mySerial.ser_write("AT+CLUCELL\r\n", 1);
					mySerial.waitForOutput();
					String retd = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
					mySerial.waitForOutput();
					String rete = mySerial.ser_write("AT+CCINFO\r\n", 1);
					mySerial.waitForOutput();
				}

				String reta = mySerial.ser_write("AT+CMSSN\r\n", 1);
				mySerial.waitForOutput();
			}
		}

	}

	public static void scan4GNetwork(JProgressBar progress) throws DisconnectException {

		mySerial.networkType = "4G";
		String ret = mySerial.ser_write("AT+CNMP=38\r\n", 1);
		mySerial.waitForOutput();

		String bandS[] = { "0x0000000000000001", "0x0000000000000004", "0x0000000000000010", "0x0000000000000040",
				"0x0000000000000080", "0x0000000000080000", "0x0000002000000000", "0x0000008000000000",
				"0x0000010000000000" };
		TwoWaySerialComm2.comandCount = 0;
		mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
		for (int i = 0; i < bandS.length; i++) {

			if (connectFlag == false) {
//				infoBox("Device is not connected. Scan will stop", "Scan Status");
				return;
			}

			String retb = mySerial.ser_write("AT+CNBP=," + bandS[i] + "\r\n", 1);
			mySerial.waitForOutput();
			String retc = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
			mySerial.waitForOutput();
			String retcc = mySerial.ser_write("AT+CMGRMI=4\r\n", 1);
			mySerial.waitForOutput();

		}

		if (mySerial.region.size() > 0) {
			List<String> mccMncList = new ArrayList<>();
			for(String regi : mySerial.region) {
				mccMncList.addAll(regionMCCMap.get(regi));
			}
			for (String mccMnc : mccMncList) {

				if (connectFlag == false) {
//					infoBox("Device is not connected. Scan will stop", "Scan Status");
					return;
				}

				String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
				mySerial.waitForOutput();
				for (int i = 0; i < bandS.length; i++) {

					if (connectFlag == false) {
//						infoBox("Device is not connected. Scan will stop", "Scan Status");
						return;
					}

					String retb = mySerial.ser_write("AT+CNBP=," + bandS[i] + "\r\n", 1);
					mySerial.waitForOutput();
					String retc = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
					mySerial.waitForOutput();
					String retcc = mySerial.ser_write("AT+CMGRMI=4\r\n", 1);
					mySerial.waitForOutput();

				}
			}
			String reta = mySerial.ser_write("AT+CMSSN\r\n", 1);
			mySerial.waitForOutput();
		}

	}

	public static void scan4GNetworkRoute(JProgressBar progress) throws DisconnectException {

		mySerial.networkType = "4G";
		String ret = mySerial.ser_write("AT+CNMP=38\r\n", 1);
		mySerial.waitForOutput();

		String bandS[] = { "0x0000000000000001", "0x0000000000000004", "0x0000000000000010", "0x0000000000000040",
				"0x0000000000000080", "0x0000000000080000", "0x0000002000000000", "0x0000008000000000",
				"0x0000010000000000" };
		TwoWaySerialComm2.comandCount = 0;
		mySerial.ser_write("AT+CMSSN" + "\r\n", 1);
		while (routeRunning) {
			for (int i = 0; i < bandS.length; i++) {

				if (connectFlag == false || routeRunning == false) {
					infoBox("Device is not connected. Scan will stop", "Scan Status");
					return;
				}

				String retb = mySerial.ser_write("AT+CNBP=," + bandS[i] + "\r\n", 1);
				mySerial.waitForOutput();
				String retc = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
				mySerial.waitForOutput();
				String retcc = mySerial.ser_write("AT+CMGRMI=4\r\n", 1);
				mySerial.waitForOutput();

			}

			if (mySerial.region.size() > 0) {
				
				List<String> mccMncList = new ArrayList<>();
				for(String regi : mySerial.region) {
					mccMncList.addAll(regionMCCMap.get(regi));
				}		
				for (String mccMnc : mccMncList) {

					if (connectFlag == false || routeRunning == false) {
						infoBox("Device is not connected. Scan will stop", "Scan Status");
						return;
					}

					String reta = mySerial.ser_write("AT+CMSSN=" + mccMnc + "\r\n", 1);
					mySerial.waitForOutput();
					for (int i = 0; i < bandS.length; i++) {

						if (connectFlag == false || routeRunning == false) {
							infoBox("Device is not connected. Scan will stop", "Scan Status");
							return;
						}

						String retb = mySerial.ser_write("AT+CNBP=," + bandS[i] + "\r\n", 1);
						mySerial.waitForOutput();
						String retc = mySerial.ser_write("AT+CSNINFO?\r\n", 1);
						mySerial.waitForOutput();
						String retcc = mySerial.ser_write("AT+CMGRMI=4\r\n", 1);
						mySerial.waitForOutput();

					}
				}
				String reta = mySerial.ser_write("AT+CMSSN\r\n", 1);
				mySerial.waitForOutput();
			}
		}

	}

	static class TimerThread extends Thread {

		protected boolean isRunning;

		protected JLabel dateLabel;
		protected JLabel timeLabel;

		protected SimpleDateFormat dateFormat = new SimpleDateFormat("EEE, d MMM yyyy");
		protected SimpleDateFormat timeFormat = new SimpleDateFormat("h:mm a");

		public TimerThread(JLabel dateLabel, JLabel timeLabel) {
			this.dateLabel = dateLabel;
			this.timeLabel = timeLabel;
			this.isRunning = true;
		}

		@Override
		public void run() {
			while (isRunning) {
				SwingUtilities.invokeLater(new Runnable() {
					@Override
					public void run() {
						Calendar currentCalendar = Calendar.getInstance();
						Date currentTime = currentCalendar.getTime();
						dateLabel.setText(dateFormat.format(currentTime));
						timeLabel.setText(timeFormat.format(currentTime));
					}
				});

				try {
					Thread.sleep(5000L);
				} catch (InterruptedException e) {
				}
			}
		}

		public void setRunning(boolean isRunning) {
			this.isRunning = isRunning;
		}

	}
}
