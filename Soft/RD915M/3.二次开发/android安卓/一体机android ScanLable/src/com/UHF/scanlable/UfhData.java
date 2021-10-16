package com.UHF.scanlable;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.Executor;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.lang.Thread;
import com.UHF.scanlable.UhfLib;

import android.R.integer;
import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;
import android.media.AudioManager;
import android.media.SoundPool;

public class UfhData {
	
	String[] epc = new String[4];
	//static SQLiteDatabase db = SQLiteDatabase.openOrCreateDatabase("/data/test.db", null);  
	static Map<String, Long> scanResult6c = new HashMap<String, Long>();
	static Map<String, Long> Result6c = new HashMap<String, Long>();
	static Map<String, byte[]> epcBytes = new HashMap<String, byte[]>();
	static int cmdnum;
	private static int scaned_num;
	private static boolean isDeviceOpen = false;
	static SoundPool soundpool = new SoundPool(1, AudioManager.STREAM_NOTIFICATION, 100);;
	static ExecutorService soundThread = Executors.newSingleThreadExecutor();
	static int soundid = soundpool.load("/etc/Scan_new.ogg", 1);
	public static boolean SoundFlag=false;//控制读卡信号
	public static boolean SoundTimer=false;//发声开关，打开了读到信号就发声
	public static Timer timer;
	
	public static String EPCID="";
	
	public static void setUfh_id(String epc)
	{
		EPCID = epc;
	}
	
	public static String GetUfh_id()
	{
		return EPCID;
	}
	public static void Set_sound(boolean flag) {
		UfhData.SoundTimer=flag;
	}

	public static boolean isDeviceOpen() {
		return isDeviceOpen;
	}

	public static void setDeviceOpen(boolean b) {
		isDeviceOpen = b;
	}
	
	public static int getScanedNum(){
		return scaned_num;
	}

	public static void readepc(){
		try{
			
			String[] lable = UhfGetData.Scan6C();
			if(lable == null) return ;
			for (int i = 0; i < lable.length; i++) {
				String key = lable[i];
				if(key == null || key.equals("")) return;
				Long num = scanResult6c.get(key) == null ? 0 : scanResult6c.get(key);
				scanResult6c.put(key, num + 1);
			}
		}catch(Exception e)
		{
			scanResult6c.clear();
		}
	}
	
	public static int EnableAlarm()
	{
		int resulrt =0x30;
		for(int index=0;index<5;index++)
		{
			byte[]epc = UhfGetData.sgScna6C();
			if(epc!=null)
			{
				if(epc[0]!=12) return 0xfb;
				byte WNum=1;
				byte ENum = 6;
				byte[]btepc=new byte[12];
				for(int m=0;m<12;m++)
					btepc[m]=epc[m+1];
				byte Mem=1;
				byte WordPtr=7;
				byte[]Password=new byte[4];
				Password[0] = Password[1]=Password[2]=Password[3]=0;
				byte[]Writedata=new byte[2];
				Writedata[0]=btepc[10];
				byte data = btepc[11];
				data = (byte)((data & 0xC0)| 1);
				Writedata[1] = data;
				resulrt = UhfGetData.Write6c(WNum, ENum, btepc, Mem, WordPtr, Writedata, Password);
				if(resulrt==0) break;
			}
		}
		return resulrt;
	}
	
	public static int DisableAlarm()
	{
		int resulrt =0x30;
		for(int index=0;index<5;index++)
		{
			byte[]epc = UhfGetData.sgScna6C();
			if(epc!=null)
			{
				if(epc[0]!=12) return 0xfb;
				byte WNum=1;
				byte ENum = 6;
				byte[]btepc=new byte[12];
				for(int m=0;m<12;m++)
					btepc[m]=epc[m+1];
				byte Mem=1;
				byte WordPtr=7;
				byte[]Password=new byte[4];
				Password[0] = Password[1]=Password[2]=Password[3]=0;
				byte[]Writedata=new byte[2];
				Writedata[0]=btepc[10];
				byte data = btepc[11];
				data = (byte)(data & 0xC0);
				Writedata[1] = data;
				resulrt = UhfGetData.Write6c(WNum, ENum, btepc, Mem, WordPtr, Writedata, Password);
				if(resulrt==0) break;
			}
		}
		return resulrt;
	}
	public static int CheckAlarm(){
		byte[]epc=null;
		for(int index =0;index<5;index++)
		{
			epc = UhfGetData.sgScna6C();
		}
		if(epc==null)//无卡
		{
			return 3;
		}
		if(epc[0]!=12)//非法卡
		{
			return 2;
		}
		byte alarm = epc[12];
		if((alarm & 0x03)==0)//不报警
		{
			soundThread.execute(UhfGetData.soundRun);
			return 0;
		}
		else if((alarm & 0x03)==1)//报警
		{
			soundThread.execute(UhfGetData.soundRun);
			return 1;
		}
		else 
			return 2;//非法卡
	}
	

	public static class UhfGetData
	{	
		private static byte Read6Cdata[]=new byte[256];
		private static int Read6CLen=-1;
		private static byte UhfVersion[]={-1,-1};
		private static byte UhfTime[]={-1};
		private static byte UhfMaxFre[]={-1};
		private static byte UhfBand[]={-1};
		
		public static int getRead6CLen(){
			return Read6CLen;
		}

		public static byte[] getRead6Cdata() {
			return Read6Cdata;
		}

		public static byte[] getUhfVersion() {
			return UhfVersion;
		}

		public static byte[] getUhfTime() {
			return UhfTime;
		}

		public static byte[] getUhfMaxFre() {
			return UhfMaxFre;
		}

		public static byte[] getUhfMinFre() {
			return UhfMinFre;
		}
		
		public static byte[] getband() {
			return UhfBand;
		}
		
		public static byte[] getUhfdBm() {
			return UhfdBm;
		}

		public static int getScan6CNum() {
			return Scan6CNum;
		}

		public static byte[] getScan6CData() {
			return Scan6CData;
		}

		public static UhfLib getUhf() {
			return uhf;
		}

		private static byte UhfMinFre[]={-1};
		private static byte UhfdBm[]={-1};
		private static int Scan6CNum=-1;
		private static byte Scan6CData[]=new byte[256];
		static UhfLib uhf = null;
		private static boolean Beep_finish=false;
		public static int OpenUhf(int tty_speed,byte addr,String comaddr,int log_swith,Context mCt )
		{	
			 String serial;
			 serial= comaddr;
			 uhf =new UhfLib(tty_speed, addr,  serial, log_swith,mCt);
			 int result=uhf.open_reader();
			 if(result!=0)return -1;
			 result=GetUhfInfo();
			 if(result==-1){
				 uhf.close_reader();
				 return -1;
			 }
			 
			 UfhData.setDeviceOpen(true);
			 if(timer == null)
			 {
				 timer = new Timer();
					timer.schedule(new TimerTask() {
						@Override
						public void run() {
							if(Beep_finish)return;
							Beep_finish=true;
							MessageBeep();
							Beep_finish=false;
						}
					}, 0, 50);
			 }
			 return 0;
			 
		}
		public static void MessageBeep()
		{
			boolean flag=SoundFlag;
			if(flag&&SoundTimer){
				soundThread.execute(soundRun);
			}
		}
		
		public static int CloseUhf()
		{
			if(uhf==null)return 0;
			if(timer != null){
				timer.cancel();
				timer = null;
			}
			uhf.close_reader();
			UfhData.setDeviceOpen(false);
			return 0;
		}
		
		
		public static int GetUhfInfo()
		{
			UhfVersion=uhf.Get_TVersionInfo();
			UhfTime=uhf.Get_ScanTime();
			UhfMaxFre=uhf.Get_dmaxfre();
			UhfMinFre=uhf.Get_dminfre();
			UhfdBm=uhf.Get_powerdBm();
			UhfBand[0] = (byte)((((UhfMaxFre[0]&255) & 0xc0) >> 4) | ((UhfMinFre[0]&255) >> 6));
			/***************************************************/
			Log.d("yl", "*********UhfVersion= = "+UhfVersion[0]+UhfVersion[1]);
			if(UhfVersion[0]==-1 && UhfVersion[1]==-1 && UhfTime[0]==-1)
			return -1;
			else
			return 0;
		}
		
		
		
		public static int SetUhfInfo(byte maxfre, byte minfre,byte power,byte scantime)
		{
			int result1=uhf.SetReader_Freq(maxfre, minfre);
			int result2=uhf.SetReader_Power(power);
			if(result1==0&&result2==0)
			{
				uhf.ReGetInfo();
				return 0;
			}
			else 
			return -1;
		}
		
			
		public static Runnable soundRun = new Runnable(){
			public void run(){
				try {
					soundpool.play(soundid, 1, 1, 0, 0, 1f);
					Thread.sleep(10);
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
		};
		
		public static byte[] sgScna6C()
		{
			byte[]pOUcharIDList=new byte[256];
			int[]pOUcharTagNum=new int[1];
			pOUcharTagNum[0]=0;
			int result=uhf.EPCC1G2_QuerySinlgCard(uhf.uhf_addr, pOUcharIDList, pOUcharTagNum);
			if(pOUcharTagNum[0]==0)
			{
				return null;
			}
			return pOUcharIDList;
		}
		public static String[] Scan6C()
		{	
			try{
				int result=uhf.EPCC1G2_ScanEPC();
				SoundFlag=false;
				if(result==0){
					SoundFlag=true;
					Scan6CNum=uhf.EPCC1G2_Inventory_POUcharTagNum();
				    Scan6CData=uhf.EPCC1G2_Inventory_pOUcharUIDList();
				    String[] lable = new String[Scan6CNum];
				    StringBuffer bf;
				    int j = 0, k;
				    String str;
				    byte[] epc;
				    Log.i("yl","num = "+ Scan6CNum + ">>>>>>"+"len = "+ Scan6CData.length);
				    for(int i = 0; i < Scan6CNum; i++){
				    	bf = new StringBuffer("");
				    	Log.i("yl","length = " + Scan6CData[j]);
				    	epc = new byte[(Scan6CData[j]) & 0xff];
				    	for(k = 0; k < ((Scan6CData[j]) & 0xff); k++){
				    		str = Integer.toHexString(Scan6CData[j+k+1] & 0xff);
				    		if(str.length() == 1){
				    			bf.append("0");
				    		}
				    		bf.append(str);
				    		epc[k] = Scan6CData[j+k+1];
				    	}
				    	lable[i] = bf.toString().toUpperCase();
				    	epcBytes.put(lable[i], epc);
				    	j = j+k+1;
				    }
				    return lable;
				}
			}catch(Exception e)
			{}
			return null;
		}
	 	private static void append(String hex) {
			// TODO Auto-generated method stub
			
		}


		public static int Read6C(byte ENum,
				byte EPC[],
				byte Mem,
				byte WordPtr,
				byte Num,
				byte Password[])
		{
			int result=uhf.ReadEPCC1G2(ENum, EPC, Mem, WordPtr, Num, Password);

			if(result==0)
			{	 
				Read6Cdata=uhf.ReadEPCC1G2_Data();
				Read6CLen =Num*2; 
				 soundThread.execute(soundRun);
				return 0;
			}
			Read6CLen=0;
			return -1;
		}
		
		public static int Write6c(byte WNum,
				byte ENum, 
				byte EPC[],
				byte Mem,
				byte WordPtr,
				byte Writedata[],
				byte Password[])
		{
			int result=uhf.WriteEPCC1G2(WNum, ENum, EPC, Mem, WordPtr, Writedata, Password);
			if (result==0)
			{
				soundThread.execute(soundRun);
			}
			return result;
		}
		   /**
	     * Convert byte[] to hex
	     * string
	     * 
	     * @param src byte[] data
	     * @return hex string
	     */
	    public static String bytesToHexString(byte[] src) {
	        StringBuilder stringBuilder = new StringBuilder("");
	        if (src == null || src.length <= 0) {
	            return null;
	        }
	        for (int i = 0; i < src.length; i++) {
	            int v = src[i] & 0xFF;
	            String hv = Integer.toHexString(v);
				if (hv.length() == 1){
					hv = '0' + hv;
				}
	            stringBuilder.append(hv);
	        }
	        return stringBuilder.toString();
	    }

	    public static String bytesToHexString(byte[] src, int offset, int length) {
	        StringBuilder stringBuilder = new StringBuilder("");
	        if (src == null || src.length <= 0) {
	            return null;
	        }
	        for (int i = offset; i < length; i++) {
	            int v = src[i] & 0xFF;
	            String hv = Integer.toHexString(v);
	            if (hv.length() == 1) {
	                stringBuilder.append(0);
	            }
	            stringBuilder.append(hv);
	        }
	        return stringBuilder.toString();
	    }


	    public static byte[] hexStringToBytes(String hexString) {  
	        if (hexString == null || hexString.equals("")) {  
	            return null;  
	        }  
	        hexString = hexString.toUpperCase();  
	        int length = hexString.length() / 2;  
	        char[] hexChars = hexString.toCharArray();  
	        byte[] d = new byte[length];  
	        for (int i = 0; i < length; i++) {  
	            int pos = i * 2;  
	            d[i] = (byte) (charToByte(hexChars[pos]) << 4 | charToByte(hexChars[pos + 1]));  
	        }  
	        return d;  
	    }   
	    private static byte charToByte(char c) {  
	        return (byte) "0123456789ABCDEF".indexOf(c);  
	    } 
	}
}
