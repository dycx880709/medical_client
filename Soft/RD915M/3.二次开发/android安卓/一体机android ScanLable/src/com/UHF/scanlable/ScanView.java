package com.UHF.scanlable;

import java.text.DecimalFormat;

import com.UHF.scanlable.UhfLib;
import com.UHF.scanlable.UfhData.UhfGetData;

import android.app.Activity;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

public class ScanView extends Activity implements OnClickListener{
	
	private Button bOpen;
	private Button bColose;
	private EditText tvVersion;
	/*private Spinner tvDminfre;
	private Spinner tvDmaxfre;*/
	private Spinner tvpowerdBm;
	
	private Button bSetting;
	private Button bRead;
	
	private Handler mHandler;
	
	UhfLib uhf = null;
	private SoundPool soundpool = null;
	private int soundid;
	private int tty_speed = 57600;
	private byte addr = (byte) 0xff; 
	private String[] strBand =new String[5]; 
    private String[] strmaxFrm =null; 
    private String[] strminFrm =null; 
    Spinner spBand;
    Spinner spmaxFrm;
	Spinner spminFrm;
	private ArrayAdapter<String> spada_Band; 
    private ArrayAdapter<String> spada_maxFrm; 
    private ArrayAdapter<String> spada_minFrm; 
	private static final String TAG = "SacnView";
	
	private static final int MSG_SHOW_PROPERTIES = 0;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		
		setContentView(R.layout.scan_view);
		initView();
		
		mHandler = new Handler(){
			@Override
			public void handleMessage(Message msg) {
				// TODO Auto-generated method stub
				super.handleMessage(msg);
				switch (msg.what) {
				case MSG_SHOW_PROPERTIES:
					String Version=String.valueOf(UhfGetData.getUhfVersion()[0])+"."+String.valueOf(UhfGetData.getUhfVersion()[1]);
					showResult(Version,UhfGetData.getband()[0],
							UhfGetData.getUhfMinFre()[0],
							UhfGetData.getUhfMaxFre()[0],
							UhfGetData.getUhfdBm()[0],UhfGetData.getUhfTime()[0]);
					bRead.setEnabled(true);
					bSetting.setEnabled(true);
					break;
				default:
					break;
				}
			}
		};
	}
	
	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
//		uhf.close_reader();
	}
	
	private void initView(){
		bOpen = (Button)findViewById(R.id.open);
		bColose = (Button)findViewById(R.id.close);
		tvVersion = (EditText)findViewById(R.id.version);
		tvVersion.setText("/dev/ttyHSL2");
		tvpowerdBm = (Spinner)findViewById(R.id.power_spinner);
		ArrayAdapter<CharSequence> adapter3 =  ArrayAdapter.createFromResource(this, R.array.Power_select, android.R.layout.simple_spinner_item);
		adapter3.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		tvpowerdBm.setAdapter(adapter3); 
		tvpowerdBm.setSelection(30, true);
		bSetting = (Button)findViewById(R.id.pro_setting);
		bRead = (Button)findViewById(R.id.pro_read);
	
		bOpen.setOnClickListener(this);
		bColose.setOnClickListener(this);
		bSetting.setOnClickListener(this);
		bRead.setOnClickListener(this);
		bRead.setEnabled(false);
		bSetting.setEnabled(false);
		////////////频段选择
		strBand[0]="User band";
		strBand[1]="Chinese band2";
		strBand[2]="US band";
		strBand[3]="Korean band";
		strBand[4]="EU band";
		
		spBand=(Spinner)findViewById(R.id.band_spinner);  
		spada_Band = new ArrayAdapter<String>(ScanView.this,  
	             android.R.layout.simple_spinner_item, strBand);  
		spada_Band.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
		spBand.setAdapter(spada_Band);  
		spBand.setSelection(0,false); 
		SetFre(1);////初始化频点
		 // 添加Spinner事件监听  
		spBand.setOnItemSelectedListener(new Spinner.OnItemSelectedListener() {  
        public void onItemSelected(AdapterView<?> arg0, View arg1,  
                int arg2, long arg3) {  
            // TODO Auto-generated method stub  
            // 设置显示当前选择的项  
            arg0.setVisibility(View.VISIBLE);  
            if(arg2==0)SetFre(0);
            if(arg2==1)SetFre(1);
            if(arg2==2)SetFre(2);
            if(arg2==3)SetFre(3);
            if(arg2==4)SetFre(4);
            //选择默认值不让执行  
        }  
        public void onNothingSelected(AdapterView<?> arg0) {  
            // TODO Auto-generated method stub  
        	}  
		});  
	
	}
	private void SetFre(int m)
	{
		if(m==1){ 
		    strmaxFrm=new String[20];
         	strminFrm=new String[20];
         	for(int i=0;i<20;i++){
         		String temp="";
         		float values=(float) (920.125 + i * 0.25);
         		temp=String.valueOf(values)+"MHz";
         		strminFrm[i]=temp;
         		strmaxFrm[i]=temp;
         	}
         	spmaxFrm=(Spinner)findViewById(R.id.max_spinner);  
         	spada_maxFrm = new ArrayAdapter<String>(ScanView.this,  
                      android.R.layout.simple_spinner_item, strmaxFrm);  
         	spada_maxFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
         	spmaxFrm.setAdapter(spada_maxFrm);  
         	spmaxFrm.setSelection(19,false);
         	
         	spminFrm=(Spinner)findViewById(R.id.min_spinner);  
         	spada_minFrm = new ArrayAdapter<String>(ScanView.this,  
                      android.R.layout.simple_spinner_item, strminFrm);  
         	spada_minFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
         	spminFrm.setAdapter(spada_minFrm);  
         	spminFrm.setSelection(0,false);
     }else if(m==2){
     	strmaxFrm=new String[50];
     	strminFrm=new String[50];
     	for(int i=0;i<50;i++){
     		String temp="";
     		float values=(float) (902.75 + i * 0.5);
     		temp=String.valueOf(values)+"MHz";
     		strminFrm[i]=temp;
     		strmaxFrm[i]=temp;
     	}
     	spmaxFrm=(Spinner)findViewById(R.id.max_spinner);  
     	spada_maxFrm = new ArrayAdapter<String>(ScanView.this,  
                  android.R.layout.simple_spinner_item, strmaxFrm);  
     	spada_maxFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
     	spmaxFrm.setAdapter(spada_maxFrm);  
     	spmaxFrm.setSelection(49,false);
     	
     	spminFrm=(Spinner)findViewById(R.id.min_spinner);  
     	spada_minFrm = new ArrayAdapter<String>(ScanView.this,  
                  android.R.layout.simple_spinner_item, strminFrm);  
     	spada_minFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
     	spminFrm.setAdapter(spada_minFrm);  
     	spminFrm.setSelection(0,false);
     }else if(m==3){
      	strmaxFrm=new String[32];
      	strminFrm=new String[32];
      	for(int i=0;i<32;i++){
      		String temp="";
      		float values=(float) (917.1 + i * 0.2);
      		temp=String.valueOf(values)+"MHz";
      		strminFrm[i]=temp;
      		strmaxFrm[i]=temp;
      	}
      	spmaxFrm=(Spinner)findViewById(R.id.max_spinner);  
      	spada_maxFrm = new ArrayAdapter<String>(ScanView.this,  
                   android.R.layout.simple_spinner_item, strmaxFrm);  
      	spada_maxFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
      	spmaxFrm.setAdapter(spada_maxFrm);  
      	spmaxFrm.setSelection(31,false);
      	
      	spminFrm=(Spinner)findViewById(R.id.min_spinner);  
      	spada_minFrm = new ArrayAdapter<String>(ScanView.this,  
                   android.R.layout.simple_spinner_item, strminFrm);  
      	spada_minFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
      	spminFrm.setAdapter(spada_minFrm);  
      	spminFrm.setSelection(0,false);
      }else if(m==4){
       	strmaxFrm=new String[15];
       	strminFrm=new String[15];
       	for(int i=0;i<15;i++){
       		String temp="";
       		float values=(float) (865.1 + i * 0.2);
       		temp=String.valueOf(values)+"MHz";
       		strminFrm[i]=temp;
       		strmaxFrm[i]=temp;
       	}
       	spmaxFrm=(Spinner)findViewById(R.id.max_spinner);  
       	spada_maxFrm = new ArrayAdapter<String>(ScanView.this,  
                    android.R.layout.simple_spinner_item, strmaxFrm);  
       	spada_maxFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
       	spmaxFrm.setAdapter(spada_maxFrm);  
       	spmaxFrm.setSelection(14,false);
       	
       	spminFrm=(Spinner)findViewById(R.id.min_spinner);  
       	spada_minFrm = new ArrayAdapter<String>(ScanView.this,  
                    android.R.layout.simple_spinner_item, strminFrm);  
       	spada_minFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
       	spminFrm.setAdapter(spada_minFrm);  
       	spminFrm.setSelection(0,false);
       }else if(m==0){
		    strmaxFrm=new String[63];
         	strminFrm=new String[63];
         	for(int i=0;i<63;i++){
         		String temp="";
         		float values=(float) (902.6 + i * 0.4);
         		temp=String.valueOf(values)+"MHz";
         		strminFrm[i]=temp;
         		strmaxFrm[i]=temp;
         	}
         	spmaxFrm=(Spinner)findViewById(R.id.max_spinner);  
         	spada_maxFrm = new ArrayAdapter<String>(ScanView.this,  
                      android.R.layout.simple_spinner_item, strmaxFrm);  
         	spada_maxFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
         	spmaxFrm.setAdapter(spada_maxFrm);  
         	spmaxFrm.setSelection(62,false);
         	
         	spminFrm=(Spinner)findViewById(R.id.min_spinner);  
         	spada_minFrm = new ArrayAdapter<String>(ScanView.this,  
                      android.R.layout.simple_spinner_item, strminFrm);  
         	spada_minFrm.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);  
         	spminFrm.setAdapter(spada_minFrm);  
         	spminFrm.setSelection(0,false);
     }
	}
	@Override
	public void onClick(View view) {
		if(view == bOpen){
			final String comaddr = tvVersion.getText().toString();
			new Thread(new Runnable() {
				
				@Override
				public void run() {
					int result=UhfGetData.OpenUhf(tty_speed, addr, comaddr, 0, null);
					if(result==0){
					UhfGetData.GetUhfInfo();
					mHandler.removeMessages(MSG_SHOW_PROPERTIES);
					mHandler.sendEmptyMessage(MSG_SHOW_PROPERTIES);
					
					}
				}
			}).start();
			
			}else if (view == bColose) {
			if(UfhData.isDeviceOpen())
			{
				UhfGetData.CloseUhf();
				ClearResult();
				bRead.setEnabled(false);
				bSetting.setEnabled(false);
			}
		}else if (view == bSetting){
			if(!UfhData.isDeviceOpen()){
				Util.showWarning(this, R.string.open_warning);
				return ;
			}
			new Thread(new Runnable() {
				@Override
				public void run() {
					// TODO Auto-generated method stub
					int MaxFre=0;
					int MinFre=0;
					int Power=0;
					int fband = spBand.getSelectedItemPosition();
					int band=0;
					if(fband==0)band=0;
					if(fband==1)band=1;
					if(fband==2)band=2;
					if(fband==3)band=3;
					if(fband==4)band=4;
					int Frequent= spminFrm.getSelectedItemPosition();
					MinFre = ((band & 3) << 6) | (Frequent & 0x3F);
					Frequent= spmaxFrm.getSelectedItemPosition();
					MaxFre = ((band & 0x0c) << 4) | (Frequent & 0x3F);
					Power = tvpowerdBm.getSelectedItemPosition();
					UhfGetData.SetUhfInfo((byte)MaxFre, (byte)MinFre, (byte)Power, (byte)0);
				}
			}).start();
		}else if (view == bRead){
			if(!UfhData.isDeviceOpen()){
				Util.showWarning(this, R.string.open_warning);
				return ;
			}
			UhfGetData.GetUhfInfo();
			mHandler.removeMessages(MSG_SHOW_PROPERTIES);
			mHandler.sendEmptyMessage(MSG_SHOW_PROPERTIES);
		}
	}
	
	private void showResult(String version,int band,int dminfre, int dmaxfre, 
			int powerdbm, int scanTime){
		//tvVersion.setText(version.toUpperCase());
		SetFre(band);
		int frequent= ((dminfre & 0x3F)&255);
		spminFrm.setSelection(frequent,true);
		spBand.setSelection(band,true);
		frequent= ((dmaxfre & 0x3F)&255);
		spmaxFrm.setSelection(frequent,true);
		tvpowerdBm.setSelection(powerdbm,true);
	}
	
	private void ClearResult(){
		//tvVersion.setText("");
		spBand.setSelection(0, true);
		SetFre(1);
		tvpowerdBm.setSelection(30,true);
	}
	
	private String getVersion(byte[] b){
		if(b!= null && b.length == 2) return b[0]+"."+b[1];
		return "";
	}
	
	private String getStr(byte[] b){
		StringBuffer sb = new StringBuffer("");
		for(int i =0; i < b.length; i++){
			sb.append(b[i]);
		}
		return sb.toString();
	}
	
}
