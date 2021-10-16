package com.UHF.scanlable;

import android.content.Context;
import android.util.Log;

public class UhfLib {
	private static final String TAG = null;
	
	private	byte TVersionInfo[]={-1,-1};
	private	byte ReaderType[]={-1};
	private	byte TrType[]={-1};
	private	byte dmaxfre[]={-1};
	private	byte dminfre[]={-1};;
	private	byte powerdBm[]={-1};
	private	byte ScanTime[]={-1};
	byte cmd[]=new byte[1];
	int len[]={-1};
	int scan_len[]={-1};
	private byte s1[]=new byte[25600];
	
	private byte state[]=new byte[1];
	private int Cardnum[]=new int[1];
	private byte pOUcharUIDList[]=new byte[25600];
	private byte pOUcharUIDList1[]=new byte[25600];
	private int  pOUcharTagNum[]=new int[1];
	private int  pOUcharTaglen[]=new int[1];
	private byte Data[]=new byte[256];
	private byte Errorcode[]=new byte[1];
	private byte read_data[]=new byte[256];
	
	private int uhf_speed;
	public byte uhf_addr;
	private Context mContext;
	private String Serial;
	private int logswitch;
	public int array_clear(byte array_clear0[])
	{
		int clear_len=array_clear0.length;
		for(int i=0;i<clear_len;i++)
		{
			array_clear0[i]=0;
		}
		return 0;
	}
	//
	public UhfLib(int tty_speed,byte addr,String serial, int log_swith,Context mCt)
	{	
		//ModulePowerOn();
		uhf_speed = tty_speed;
		uhf_addr = addr;
		mContext = mCt;
		Serial = serial;
		logswitch = log_swith;
	}

public int open_reader()
  {
	  	int reply1=1;
	  	reply1=ConnectReader( Serial,uhf_speed,1);
	  	
	  	if(reply1==0)
	  	{
	  		GetReaderInformation(uhf_addr, TVersionInfo, ReaderType, TrType, dmaxfre, dminfre, powerdBm, ScanTime);
	  		return 0;
	  	}
  		return -1;
  	}
public int ReGetInfo()
{
	
	int result =GetReaderInformation(uhf_addr, TVersionInfo, ReaderType, TrType, dmaxfre, dminfre, powerdBm, ScanTime);
	if  (result==0)
	return 0;
	return -1;
}

  public int close_reader()
  {
	  	
	  DisconnectReader();
	  return 0;
  }
  public int SetReader_Newaddress(byte newaddr)
  {
	  if(WriteAddress(uhf_addr,newaddr)==0)
	  {
		  uhf_addr=newaddr;
		  return 0;
	  }
	  return -1;
	 
  }
  

  public int SetReader_ScanTime(byte scantime)
  {
	
		 if(WriteScanTime(uhf_addr,scantime)==0)
		 {
			 ScanTime[0]=scantime;
			  return 0;
		 }
	  return -1;
  }

  public int SetReader_Power(byte power)
  {
	  if(WritePower(uhf_addr,power)==0)
	  {
		  powerdBm[0]=power;
		  return 0;
	  
	  }
	  	return -1;
  }
  
 
  public int SetReader_Freq(byte maxfre, byte minfre)
  {
	  if(WriteFreq(uhf_addr,maxfre,minfre)==0)
	  {  	
		  dmaxfre[0]=maxfre;
		  dminfre[0]=minfre;
		  return 0;
	  }
		  return -1;
  }
  

  public int SetReader_BaudRate(int fbaud)
  {	  byte fbaud1;
	  if(fbaud==9600)
		  fbaud1=0x00;
	  else if(fbaud==19200)
		  fbaud1=0x01;
	  else if(fbaud==38400)
		  fbaud1=0x02;
	  else if(fbaud==57600)
		  fbaud1=0x05;
	  else if(fbaud==115200)
		  fbaud1=0x06;
	  else
		  return -1;
	  if(WriteBaudRate(uhf_addr,fbaud1)==0)
	  {
		  uhf_speed=fbaud;
		  return 0;
	  }
	  return -1;
  }
  
  public byte[] Get_TVersionInfo()
  {
		  return TVersionInfo;
	  
  }
  public byte[] Get_ReaderType()
  {
		  return ReaderType;
	  
  }
  public  byte[] Get_TrType()
  {
		  return TrType;
	  
  }
  public byte[] Get_dmaxfre()
  {
		  return dmaxfre;
	  
  }
  public byte[] Get_dminfre()
  {
		  return dminfre;
	  
  }
  public byte[] Get_powerdBm()
  {
		  return powerdBm;
	  
  } 
  public byte[] Get_ScanTime()
  {
		  return ScanTime;
	  
  } 
 
  public int EPCC1G2_ScanEPC()
  {  
	  byte s3[]={0x00};
	  byte s2=0x00;
	  int reply=-1;
	  array_clear(pOUcharUIDList1);
	  reply=EPCC1G2_Inventory(uhf_addr,
				pOUcharUIDList1,		
				pOUcharTagNum,
				pOUcharTaglen);  
	  if(reply==0)
		  return 0;
	  return -1;

	}

public int EPCC1G2_Inventory_Len()
{
			return pOUcharTaglen[0];

}
public byte[] EPCC1G2_Inventory_pOUcharUIDList()
{
		
			return pOUcharUIDList1;
} 
  
public int EPCC1G2_Inventory_POUcharTagNum()
{
			return pOUcharTagNum[0];
}  


public int ReadEPCC1G2(
		byte ENum,
		byte EPC[],
		byte Mem,
		byte WordPtr,
		byte Num,
		byte Password[])
{
	array_clear(Data);
	if(EPCC1G2_ReadCard(uhf_addr,
			ENum,
			EPC,
			Mem,
			WordPtr,
			Num,
			Password,
			Data,
			Errorcode)==0)
		return 0;
	return -1;
	
}

public byte[] ReadEPCC1G2_Data()
{
			return Data;
}

public byte ReadEPCC1G2_Errorcode()
{
			return Errorcode[0];

}



public byte WriteEPCC1G2(
		byte WNum,
		byte ENum, 
		byte EPC[],
		byte Mem,
		byte WordPtr,
		byte Writedata[],
		byte Password[])
{
			int reply=-1;
			reply=EPCC1G2_WriteCard (uhf_addr,
			WNum,
			ENum, 
			EPC,
		    Mem,
			WordPtr,
			Writedata,
			Password,
			Errorcode);
			if(reply==0)
				return Errorcode[0];
			return -1;
}

static{
 	System.loadLibrary("Uhf_jni");
}
//5 test function	
static native int fsSayHello();
static native int fsLedOpen();
static native int fsLedOn();
static native int fsLedOff();
static native int ConnectReader(String serial,int speed,int log_swith);
static native int DisconnectReader();
static native int WriteScanTime(byte addr, byte ScanTime);
static native int WritePower(byte addr, byte power);
static native int WriteAddress(byte addr, byte newAddr);
static native int WriteFreq (byte addr, byte maxfre, byte minfre);
static native int WriteBaudRate (byte addr, byte fbaud);
static native int GetReaderInformation(byte addr,
				byte TVersionInfo[],
				byte ReaderType[],
				byte TrType[],
				byte dmaxfre[],
				byte dminfre[],
				byte powerdBm[],
				byte ScanTime[]);


static native int EPCC1G2_Inventory(byte addr,
									byte pOUcharIDList[],	
									int pOUcharTagNum[],
									int TotalLen[]);
		


static native int EPCC1G2_ReadCard(byte addr,
		byte ENum,
		byte EPC[],
		byte Mem,
		byte WordPtr,
		byte Num,
		byte Password[],
		byte Data[],
		byte Errorcode[]);

static native int EPCC1G2_WriteCard (byte addr,
	byte WNum,
	byte ENum, 
	byte EPC[],
	byte Mem,
	byte WordPtr,
	byte Writedata[],
	byte Password[],
	byte Errorcode[]);

static native int  EPCC1G2_QuerySinlgCard(byte addr,
		byte pOUcharIDList[],
		int pOUcharTagNum[]);

static native int  EPCC1G2_WriteEPC(byte addr,
		byte ENum,
		byte Password[],
		byte EPC[],
		byte[] errorcode);

}

