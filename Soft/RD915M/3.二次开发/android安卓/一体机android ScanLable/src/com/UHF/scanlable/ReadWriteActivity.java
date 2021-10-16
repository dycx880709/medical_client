package com.UHF.scanlable;

import com.UHF.scanlable.UfhData.UhfGetData;

import android.R.integer;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ActivityGroup;
import android.content.Intent;
import android.graphics.Color;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Bundle;
import android.support.v4.widget.SimpleCursorAdapter.ViewBinder;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnFocusChangeListener;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import java.util.concurrent.Executor;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ReadWriteActivity extends Activity implements OnClickListener, OnItemSelectedListener{
	
	
	EditText edENum0;
	int selectedEd = 3;
	int selectedWhenPause = 0;
	
	Spinner c_mem;
	EditText c_wordPtr;
	EditText c_len;
	EditText c_pwd;
	EditText c_ptr;
	
	EditText b_id;
	EditText b_addr;
	EditText b_num;
	
	EditText content;
	Button rButton;
	Button wButton;
	
	private static final int CHECK_W_6B = 0;
	private static final int CHECK_R_6B = 1;
	private static final int CHECK_W_6C = 2;
	private static final int CHECK_R_6C = 3;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		Log.i("zhouxin", ">>>>>>>>>>>>>>>>>>>>>rw oncreate" + this);
		
		setContentView(R.layout.rw_6c);
		initView();
	}
	
	@Override
	protected void onResume() {
		// TODO Auto-generated method stub
		Log.i("zhouxin",">>>>>>>>>>>>>>>>>>>>>>rw onResume");
		content.setText("");
		edENum0.setText(UfhData.GetUfh_id());
		super.onResume();
	}
	
	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
	}
	
	@Override
	protected void onDestroy() {
		// TODO Auto-generated method stub
		Log.i("zhouxin", ">>>>>>>>>>>>>>>>>>>>>rw onDestroy");
		super.onDestroy();
	}
	
	private void initView(){
		edENum0 = (EditText)findViewById(R.id.epc0);
		c_mem = (Spinner)findViewById(R.id.mem_spinner);
		ArrayAdapter<CharSequence> adapter =  ArrayAdapter.createFromResource(this, R.array.men_select, android.R.layout.simple_spinner_item);
		adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
		c_mem.setAdapter(adapter); 
		c_mem.setSelection(3, true);
		c_mem.setOnItemSelectedListener(this);
		
		c_wordPtr = (EditText)findViewById(R.id.et_wordptr);
		c_wordPtr.setText("0");
		c_len = (EditText)findViewById(R.id.et_length);
		c_len.setText("4");
		c_pwd = (EditText)findViewById(R.id.et_pwd);
		c_pwd.setText("00000000");
		content = (EditText)findViewById(R.id.et_content_6c);
		rButton = (Button)findViewById(R.id.button_read_6c);
		wButton = (Button)findViewById(R.id.button_write_6c);
		rButton.setOnClickListener(this);
		wButton.setOnClickListener(this);
	}
	

	@Override
	public void onClick(View view) {
		Log.i("zhouxin", "----onclick----");
		if(!UfhData.isDeviceOpen()){
			Toast.makeText(this, R.string.open_warning, Toast.LENGTH_LONG).show();
			return;
		}
		String temp="";
		if(view == wButton){
		    temp = content.getText().toString();
			int result=UhfGetData.Write6c(
//					UhfGetData.hexStringToBytes(c_len.getText().toString())[0], 
					(byte)(int)Integer.valueOf(c_len.getText().toString()),
					(byte)(UfhData.epcBytes.get(edENum0.getText().toString()).length/2), 
					UfhData.epcBytes.get(edENum0.getText().toString()), 
					(byte)selectedEd, 
//					UhfGetData.hexStringToBytes(c_wordPtr.getText().toString())[0],
					(byte)(int)Integer.valueOf(c_wordPtr.getText().toString()),
					UhfGetData.hexStringToBytes(temp), 
					UhfGetData.hexStringToBytes(c_pwd.getText().toString()));
		}else if(view == rButton){
			content.setText("");
			Log.i("zhouxin",">>>"+UhfGetData.hexStringToBytes(c_pwd.getText().toString()).length);
			UhfGetData.Read6C((byte)(UfhData.epcBytes.get(edENum0.getText().toString()).length/2),
					UfhData.epcBytes.get(edENum0.getText().toString()),
					(byte)selectedEd,
					Byte.valueOf(c_wordPtr.getText().toString()),
					Byte.valueOf(c_len.getText().toString()),
					UhfGetData.hexStringToBytes(c_pwd.getText().toString()));
		    temp=UhfGetData.bytesToHexString(UhfGetData.getRead6Cdata(),0,UhfGetData.getRead6CLen()).toUpperCase();
			int m= UhfGetData.getRead6CLen();
			if(m==0){
				content.setText("");
			}
			else{
				content.setText(temp.toUpperCase());
			}
		}
	}

	@Override
	public void onItemSelected(AdapterView<?> arg0, View arg1, int position,
			long arg3) {
		// TODO Auto-generated method stub
		selectedEd = position;
	}

	@Override
	public void onNothingSelected(AdapterView<?> arg0) {
		// TODO Auto-generated method stub
		
	}


}
