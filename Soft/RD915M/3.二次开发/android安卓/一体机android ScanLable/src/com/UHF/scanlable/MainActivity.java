package com.UHF.scanlable;

import com.UHF.scanlable.UfhData.UhfGetData;

import android.os.Bundle;
import android.app.Activity;
import android.app.TabActivity;
import android.content.Intent;
import android.view.Menu;
import android.view.Window;
import android.widget.TabHost;

public class MainActivity extends TabActivity {
	
	private String[] tableMenu = {"Scan","6C","6B"};
	private Intent[] tableIntents;
	
	private TabHost myTabHost;
		
	public static final String EXTRA_MODE = "mode";
	public static final String EXTRA_EPC = "epc";
	
	public static final String TABLE_SCAN = "CMD";
	public static final String TABLE_AL = "Read/Write";
	public static final String TABLE_6C = "EPC";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		setContentView(R.layout.activity_main);
		
		myTabHost = getTabHost();
		Intent intent0 = new Intent(this,ScanView.class);
		Intent intent1 = new Intent(this,ScanModeGroup.class);
		intent1.putExtra(EXTRA_MODE, TABLE_6C);
		Intent intent2 = new Intent(this,ReadWriteActivity.class);
		intent2.putExtra(EXTRA_MODE, TABLE_AL);
		
		TabHost.TabSpec tabSpec0 = myTabHost.newTabSpec(TABLE_SCAN).setIndicator(TABLE_SCAN).setContent(intent0);
		TabHost.TabSpec tabSpec1 = myTabHost.newTabSpec(TABLE_6C).setIndicator(TABLE_6C).setContent(intent1);
		TabHost.TabSpec tabSpec2 = myTabHost.newTabSpec(TABLE_AL).setIndicator(TABLE_AL).setContent(intent2);
		
		myTabHost.addTab(tabSpec0);
		myTabHost.addTab(tabSpec1);
		myTabHost.addTab(tabSpec2);
		
		myTabHost.setCurrentTab(0);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
//		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	
	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
		//if(UhfGetData.)
		UhfGetData.CloseUhf();
	}

}
