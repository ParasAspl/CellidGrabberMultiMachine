package cellid.utils;

//import gnu.io.CommPort;
//import gnu.io.CommPortIdentifier;
//import gnu.io.SerialPort;
//import gnu.io.SerialPortEvent;
//import gnu.io.SerialPortEventListener;

import java.awt.Color;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.security.Timestamp;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.*;

import javax.swing.JTextPane;
import javax.swing.table.DefaultTableModel;
import javax.swing.text.BadLocationException;
import javax.swing.text.Style;
import javax.swing.text.StyleConstants;
import javax.swing.text.StyledDocument;

public class TwoWaySerialComm2 {
	public InputStream in1;
	public OutputStream out1;
	public CommPort commPort;
	public SerialPort serialPort;
	public String port_name;
	public static int comandCount;
	public DefaultTableModel dtm;
	public static StringBuilder doc;
	public static boolean scanRunning;
	public String date;
	Map<String, String[]> regionCircleMap;
	public String region = null;
	public String networkType = "2G";
	DateFormat df;
	Set<String> set2G;
	Set<String> set3G;
	Set<String> set4G;
	public TwoWaySerialComm2(DefaultTableModel dtm, Map<String, String[]> regionCircleMap)// , JSObject win)
	{
		df = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
		this.doc = new StringBuilder();
		this.dtm = dtm;
		this.regionCircleMap = regionCircleMap;
		set2G = new HashSet<>();
		set3G = new HashSet<>();
		set4G = new HashSet<>();
		scanRunning = false;
	}
	
	public void resetSetValues() {
		set2G = new HashSet<>();
		set3G = new HashSet<>();
		set4G = new HashSet<>();
	}

	
	public String connect(String portName, int speed) throws Exception {
		String ret = "connect_fail";

		CommPortIdentifier portIdentifier = CommPortIdentifier
				.getPortIdentifier(portName);
		System.out.println("PortIdentifier is " + portIdentifier + "\r\n\r\n");
		if (portIdentifier.isCurrentlyOwned()) {
			System.out.println("Error: Port is currently in use");
			String s1 = "\nError: " + portName + " port is currently in use.\n";
			// todo : show a popup message here with s1
			ret = "connect_port_in_use";
		} else {
			// String s = "Before opening portIdentifier open\r\n";
			try {
				this.commPort = portIdentifier.open(getClass().getName(), 2000);
			} catch (Exception e) {
				System.out.println(e.getMessage());

				String s1 = portName
						+ "  is currently in use. Please close the application that is using this port and connect again."
						+ "\r\n";
				// todo : show a popup message here with s1
				return "connect_port_in_use";
			}
			System.out.println("commPort is " + this.commPort + "\r\n");
			if ((this.commPort instanceof SerialPort)) {
				this.serialPort = ((SerialPort) this.commPort);
				this.port_name = portIdentifier.getName();

				this.serialPort.setSerialPortParams(speed, 8, 1, 0);

				InputStream in = this.serialPort.getInputStream();
				OutputStream out = this.serialPort.getOutputStream();
				this.in1 = in;
				this.out1 = out;

				this.serialPort
						.addEventListener(new TwoWaySerialComm2.SerialReader(
								this.in1));
				this.serialPort.notifyOnDataAvailable(true);
				// String s1 = "Connected to port " + portName +
				// " at baud rate " + speed + " bps.\r\n\r\n";

				ret = "connect_sucess";
			} else {
				System.out
						.println("Error: Only serial ports are handled by this example.");

				ret = "connect_error";
				String s1 = "Error connecting to " + portName + ".\r\n";
				// todo : show a popup message here with s1
			}
		}
		return ret;
	}

	public String listPorts() {
		List<String> port_name = new ArrayList<String>();
		String tmp = "";

		// int i = 0;
		@SuppressWarnings("unchecked")
		Enumeration<CommPortIdentifier> portEnum = CommPortIdentifier
				.getPortIdentifiers();

		// Enumeration portList = CommPortIdentifier.getPortIdentifiers();
		while (portEnum.hasMoreElements()) {
			CommPortIdentifier portIdentifier = (CommPortIdentifier) portEnum
					.nextElement();
			if (getPortTypeName(portIdentifier.getPortType()) == "Serial") {
				port_name.add(portIdentifier.getName());
				String tmp2 = portIdentifier.getName() + "#";
				tmp = tmp + tmp2;
			}
			System.out.println(portIdentifier.getName() + " - "
					+ getPortTypeName(portIdentifier.getPortType()));
		}
		// i = port_name.size();

		return tmp;
	}

	public String disconnect() {
		String ret = "disconnect_fail";
		if (this.serialPort != null) {
			try {
				this.in1.close();
				this.out1.close();
			} catch (IOException localIOException) {
			}
			String s1 = "Disconnecting port " + this.port_name + ".\r\n\r\n";
			// todo : show a popup message here with s1
			this.serialPort.close();
			this.serialPort = null;

			ret = "disconnect_success";
		} else {
			String s1 = "No port is connected.\r\n\r\n";
			// todo : show a popup message here with s1
			ret = "disconnect_port_not_selected";
		}
		return ret;
	}

	static String getPortTypeName(int portType) {
		switch (portType) {
			case 3:
				return "I2C";
			case 2:
				return "Parallel";
			case 5:
				return "Raw";
			case 4:
				return "RS485";
			case 1:
				return "Serial";
		}
		return "unknown type";
	}

	public static void waitForOutput() {
		waitForOutput(10);
	}

	public static void waitForOutput(int seconds) {
		try {
			int counter = 0;
			while(counter < seconds) {
				if(TwoWaySerialComm2.comandCount == 0) {
					break;
				}
				Thread.sleep(1000L);
				counter++;
			}
			if(counter == seconds) {
				System.out.println("Counter has reset due to timeout");
				TwoWaySerialComm2.comandCount = 0;
			}
		} catch (InterruptedException localInterruptedException) {
		}
	}

	public synchronized String ser_write(String str, int type) throws DisconnectException {
		String ret = "submit_fail";
		
		if (this.serialPort != null) {
			try {
				System.out.println(str);
//				System.out.println("Test 2 succssful");
				this.out1.write(str.getBytes());
/*
				if (type == 1) {
					String[] t5 = str.split("\r\n");
					last_cmd = t5[0];
				}
*/
				TwoWaySerialComm2.comandCount = 1;
				ret = "submit_data_written";
			} catch (Exception e) {
				e.printStackTrace();
			}
		} else {
			String s1 = "No port is connected.\r\n\r\n";
			// todo : show a popup message here with s1
			ret = "submit_port_not_connected";
			throw new DisconnectException("Device is disconnected");
		}
		return ret;
	}

	public String check_port_connected() {
		String ret = "NULL";
		if (this.serialPort != null) {
			ret = this.serialPort.getName();
		}
		return ret;
	}

	public class SerialReader implements SerialPortEventListener {
		private InputStream in;
		private byte[] portBuffer = new byte[1024];
		private String fileBuffer;
		// public JSObject win;
		public int winflag = 1;

		public SerialReader(InputStream in)// , JSObject win)
		{
			this.in = in;
			// this.win = win;
		}

		public synchronized void serialEvent(SerialPortEvent arg0) {
			String delimiter = "\r\n";
			try {
				int len = 0;
				int data;
				while ((data = this.in.read()) > -1) {

					this.portBuffer[(len++)] = ((byte) data);
					if (data == 10) {
						break;
					}
				}
				this.fileBuffer = new String(this.portBuffer, 0, len);
//				Date today = new Date();
//				date = today.toString();
				System.out.print(this.fileBuffer.toString());
				String tstr = this.fileBuffer.toString();

				if (tstr.trim().equalsIgnoreCase("OK") || tstr.trim().equalsIgnoreCase("ERROR")) {
					System.out.println("resetting to producer buffer");
					TwoWaySerialComm2.comandCount=0;
				}

				try {
				if(scanRunning) {
					if((!(tstr.contains("CCINFO") || tstr.contains("ccinfo"))) && ((tstr.contains("arfcn:") && tstr.contains("mcc:")) || (tstr.contains("ARFCN:") && tstr.contains("MCC:"))) && networkType.equals("2G")) {
						parseEventCNSVS2G(tstr);
					}
					if(tstr.contains("SCELL") || tstr.contains("scell")) {
						if(networkType.equalsIgnoreCase("2G")) {
							parseEventCCINFO2G(tstr);
						} else if(networkType.equalsIgnoreCase("3G")) {
							if(tstr.contains("CSNINFO")) {
								parseEventCSNINFO3G(tstr);
							} else {
								parseEventCCINFO3G(tstr);
							}
						} else {
							System.out.println("Error: non binded ccinfo");
						}
					}
					if(tstr.contains("Serving_Cell")|| (tstr.contains("CSNINFO") && networkType.equals("4G"))) {
						
						if(tstr.contains("Serving_Cell")) {
							parseEvent4G(tstr);
						}
						
						if(tstr.contains("CSNINFO") && (tstr.contains("SCELL") || tstr.contains("scell"))) {
							parseEventCSNINFO4G(tstr);
						}
					}
				} else {
					System.out.println("Error: scan is not running");
				}
				} catch (Exception e) {
					e.printStackTrace();
				}
				
				try {
					doc.append(tstr);
				} catch (Exception e) {
					e.printStackTrace();
				}
				return;
			} catch (IOException e) {
				e.printStackTrace();
				System.exit(-1);
			}
		}
		
		public String getNetworkStrength2G(String dbM) {
			Integer val = Integer.parseInt(dbM);
			
			if(val == 0) {
				return "Poor";
			} else if(val >= -70) {
				return "Excellent";
			} else if(val < -70 && val >= -85) {
				return "Good";
			} else if(val < -85 && val >= -100) {
				return "Fair";
			} else if(val < -100 && val >= -110) {
				return "Poor";
			} else {
				return "Poor";
			}
			
		}
		
		public String getNetworkStrength4G(String dbM) {
			Integer val = Integer.parseInt(dbM);
			
			if(val == 0) {
				return "Poor";
			} else if(val >= -80) {
				return "Excellent";
			} else if(val < -80 && val >= -90) {
				return "Good";
			} else if(val < -90 && val >= -100) {
				return "Fair";
			} else {
				return "Poor";
			}
			
		}

		public void parseEventCNSVS2G(String tstr) {
			
			System.out.println("Parsing 2G event: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;
			str[3] = getData(split[3]);
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = getData(split[4]);
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				System.out.println(opertorCircle.length);
				str[1] = opertorCircle[1];
				if(region == null) {
					region = opertorCircle[1];
				}
				str[2] = opertorCircle[0];
			} else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = getData(split[5]);
			str[6] = getData(split[6]);
			str[7] = getData(split[6]);
			
			
			StringBuilder sb4=new StringBuilder(str[4]);  
		    sb4.reverse();			
			if(str[2].equals("Cellone")) {
				str[8] = "0454-" + sb4 + "-" + 
						Integer.toHexString(Integer.parseInt(str[5])) + 
						"-" + Integer.toHexString(Integer.parseInt(str[6]));
			} else {
				str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6];
			}
			str[9] = getData(split[0]);
			str[10] = "NA";
			str[11] = networkType;
			str[12] = getData(split[1]);
			str[13] = getData(split[2]);
			str[14] = getNetworkStrength2G(str[13]);

			if(set2G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set2G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}
				dtm.addRow(str);
			}

		}
		
		public void parseEventCCINFO2G(String tstr) {
			
			System.out.println("Parsing 2G event CCINFO: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;
			str[3] = getData(split[2]);
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = getData(split[3]);
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				System.out.println(opertorCircle.length);
				str[1] = opertorCircle[1];
				if(region == null) {
					region = opertorCircle[1];
				}
				str[2] = opertorCircle[0];
			}else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = getData(split[4]);
			str[6] = getData(split[5]);
			str[7] = getData(split[5]);
			
			StringBuilder sb4=new StringBuilder(str[4]);  
		    sb4.reverse();  
			
			if(str[2].equals("Cellone")) {
				str[8] = "0454-" + sb4 + "-" + 
						Integer.toHexString(Integer.parseInt(str[5])) + 
						"-" + Integer.toHexString(Integer.parseInt(str[6]));
			} else {
				str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6];
			}
			str[9] = getData(split[1]);
			str[10] = "NA";
			str[11] = networkType;
			str[12] = getData(split[6]);
			str[13] = getData(split[7]).split("d")[0];
			str[14] = getNetworkStrength2G(str[13]);

			if(set2G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set2G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}

				dtm.addRow(str);
			}

		}


		public void parseEventCSNINFO3G(String tstr) {
			
			System.out.println("Parsing 3G event CSNINFO: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;
			String mccMnc = split[3].trim();
			String mccMncSplit[] = mccMnc.split("-");
			str[3] = mccMncSplit[0].trim();
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = mccMncSplit[1].trim();
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				str[1] = opertorCircle[1];
				str[2] = opertorCircle[0];
				if(region == null) {
					region = opertorCircle[1];
				}
			} else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = split[4].trim();
			str[6] = split[5].trim();
			str[7] = split[5].trim();
			
			StringBuilder sb4=new StringBuilder(str[4]);  
		    sb4.reverse();
		    
		    if(str[2].equals("Cellone")) {
				str[8] = "0454-" + sb4 + "-" + 
						Integer.toHexString(Integer.parseInt(str[5])) + 
						"-" + Integer.toHexString(Integer.parseInt(str[6]));
			} else {
				str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6];
			}
			str[9] = split[8].trim();
			str[10] = "NA";
			str[11] = networkType;
			str[12] = split[7].trim();
			str[13] = "-"+split[11].trim();
			str[14] = getNetworkStrength2G(str[13]);
			
			if(set3G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set3G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}
				dtm.addRow(str);
			}			

		}
		
		public void parseEventCCINFO3G(String tstr) {
			
			System.out.println("Parsing 3G event CCINFO: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;
			str[3] = getData(split[2]);
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = getData(split[3]);
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				System.out.println(opertorCircle.length);
				str[1] = opertorCircle[1];
				if(region == null) {
					region = opertorCircle[1];
				}
				str[2] = opertorCircle[0];
			} else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = getData(split[4]);
			str[6] = getData(split[5]);
			str[7] = getData(split[5]);
			
			StringBuilder sb4=new StringBuilder(str[4]);  
		    sb4.reverse();
		    
		    if(str[2].equals("Cellone")) {
				str[8] = "0454-" + sb4 + "-" + 
						Integer.toHexString(Integer.parseInt(str[5])) + 
						"-" + Integer.toHexString(Integer.parseInt(str[6]));
			} else {
				str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6];
			}
			str[9] = getData(split[1]);
			str[10] = "NA";
			str[11] = networkType;
			str[12] = getData(split[6]);
			str[13] = getData(split[10]).split("d")[0];
			str[14] = getNetworkStrength2G(str[13]);
			
			if(set3G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set3G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}
				dtm.addRow(str);
			}

		}

		public void parseEvent4G(String tstr) {
			
			System.out.println("Parsing 4G event: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;

			str[3] = split[2].trim(); // mcc
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = split[3].trim(); //mnc
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				str[1] = opertorCircle[1];
				str[2] = opertorCircle[0];
				if(region == null) {
					region = opertorCircle[1];
				}
			} else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = split[4].trim(); // lac
			str[6] = split[6].trim(); //eci
			
			int eci = Integer.parseInt(str[6]);
			int enb = eci/256;
			String enb_str = enb + "";
			String eci_hex = Integer.toHexString(eci);
			String eci_hex_last = eci_hex.substring(eci_hex.length() - 2, eci_hex.length());
			int sectorId = Integer.parseInt(eci_hex_last, 16);
			
			System.out.println("operator str :" + str[2].toLowerCase()+";");
			switch(str[2].toLowerCase()) {
				case "jio" :
					enb_str=eci_hex;
					str[7] = enb_str;
					break;
				default :
					str[7] = enb + "" + sectorId; // cell id	
			}
			
			switch(str[2].toLowerCase()) {
				case "airtel" :
					System.out.println("operator : airtel");
					str[8] = str[3] + "-" + str[4] + "-" + str[6];
					break;
				case "reliance" :
					System.out.println("operator : reliance");
					str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[7];
					break;
				case "jio" :
					System.out.println("operator : jio");
					int length = str[3].length() + str[4].length() + enb_str.length();
					String zero = "";
					int diff = 13  - length;
					while(diff > 0) {
						zero = zero + "0";
						diff--;
					}
					str[8] = str[3] + "-" + str[4] + "-" + zero + enb_str;
					break;
				case "vodafone idea" :
					str[8] = str[3] + "-" + str[4] + "-" + str[7];
				default :
					System.out.println("operator : default");
					/* str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6]; */
					str[8] = str[3] + "-" + str[4] + "-" + str[7];
					break;
			}

			str[9] = split[1].trim();
			str[10] = enb_str;
			str[11] = networkType;
			str[12] = split[11].trim();
			str[13] = split[12].trim();
			str[14] = getNetworkStrength4G(str[13]);


			if(set4G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set4G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}
				dtm.addRow(str);
			}

		}
		
		public void parseEventCSNINFO4G(String tstr) {
			
			System.out.println("Parsing 4G event CSNINFO: " + tstr);
			
			String split[] = tstr.split(",");
			String str[] = new String[15];
			Date dateobj = new Date();
			String dateStr = df.format(dateobj);

			str[0] = dateStr;
			String mccMnc = split[3].trim();
			String mccMncSplit[] = mccMnc.split("-");
			str[3] = mccMncSplit[0].trim();
			if(str[3].length() == 1) {
				str[3] = "0" + str[3];
				System.out.println("Updated str[3]" + str[3]);
			}
			str[4] = mccMncSplit[1].trim();
			if(str[4].length() == 1) {
				str[4] = "0" + str[4];
				System.out.println("Updated str[4]" + str[4]);
			}
			
			String opertorCircle[] = regionCircleMap.get(str[3] + "-" + str[4]);
			if(opertorCircle != null && opertorCircle.length > 0) {
				str[1] = opertorCircle[1];
				str[2] = opertorCircle[0];
				if(region == null) {
					region = opertorCircle[1];
				}
			} else {
				System.out.println("No region found:" + str[3]+"-"+str[4] + "length=" + str[3].length());
			}
			str[5] = split[4].trim();
			str[6] = split[5].trim();
			
			int eci = Integer.parseInt(str[6]);
			int enb = eci/256;
			String enb_str = enb + "";
			String eci_hex = Integer.toHexString(eci);
			String eci_hex_last = eci_hex.substring(eci_hex.length() - 2, eci_hex.length());
			int sectorId = Integer.parseInt(eci_hex_last, 16);
			
			System.out.println("operator str :" + str[2].toLowerCase()+";");
			switch(str[2].toLowerCase()) {
				case "jio" :
					enb_str=eci_hex;
					str[7] = enb_str;
					break;
				default :
					str[7] = enb + "" + sectorId; // cell id	
			}
			
			switch(str[2].toLowerCase()) {
				case "airtel" :
					System.out.println("operator : airtel");
					str[8] = str[3] + "-" + str[4] + "-" + str[6];
					break;
				case "reliance" :
					System.out.println("operator : reliance");
					str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[7];
					break;
				case "jio" :
					System.out.println("operator : jio");
					int length = str[3].length() + str[4].length() + enb_str.length();
					String zero = "";
					int diff = 13  - length;
					while(diff > 0) {
						zero = zero + "0";
						diff--;
					}
					str[8] = str[3] + "-" + str[4] + "-" + zero + enb_str;
					break;
				case "vodafone idea" :
					str[8] = str[3] + "-" + str[4] + "-" + str[7];
				default :
					System.out.println("operator : default");
					//str[8] = str[3] + "-" + str[4] + "-" + str[5] + "-" + str[6];
					str[8] = str[3] + "-" + str[4] + "-" + str[7];
					
					break;
			}
			str[9] = split[8].trim();
			str[10] = enb_str;
			str[11] = networkType;
			str[12] = split[6].trim();
			str[13] = split[12].trim();
			str[14] = getNetworkStrength4G(str[13]);
			if(set4G.contains(str[7] + "-" + str[9])) {
				
			} else {
				set4G.add(str[7] + "-" + str[9]);
				if(filterEvent(str)) {
					return; // skip this event
				}
				dtm.addRow(str);
			}

		}

		
		public String getData(String value) {
			if(value == null || value.length() == 0) {
				return "";
			}
			String str[] = value.split(":");
			if(str.length == 0) {
				return "";
			}
			return str[1].trim();
		}
		
		public boolean filterEvent(String str[]) {
			if(str[9].equalsIgnoreCase("0") || str[9].equalsIgnoreCase("-1")) {
				return true;
			}
			return false;
		}
	}

	public static class SerialWriter implements Runnable {
		OutputStream out;

		public SerialWriter(OutputStream out) {
			this.out = out;
		}

		public void run() {
			try {
				int c = 0;
				while ((c = System.in.read()) > -1) {
					this.out.write(c);
				}
			} catch (IOException e) {
				e.printStackTrace();
				System.exit(-1);
			}
		}
	}
}