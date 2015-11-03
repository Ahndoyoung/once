package com.example.Once;

import android.app.Activity;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.transition.TransitionManager;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;


import java.io.*;
import java.net.Socket;

/**
 * Created by ahndoyoung on 2015. 11. 3..
 */
public class LoginActivity extends Activity {

    public EditText etIP;
    public TextView tvMsg;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);
        initres();

    }

    public void initres(){

        etIP = (EditText) findViewById(R.id.etIP);
        etIP.setText(Preference.getString(this, "ip"));
        tvMsg = (TextView) findViewById(R.id.tvMsg);
    }

    public void Connect(View v) {
        Preference.putString(this, "ip", etIP.getText().toString());
        new LoginAsyncTask().execute(null, null, null);

    }

    class LoginAsyncTask extends AsyncTask<Void, Void, Void> {

        int state = -1;
        @Override
        protected Void doInBackground(Void... params) {
            state = -1;
            try {

                Connector.ip = etIP.getText().toString();
                Connector.getInstance();
                state = 0;
            } catch (IOException err) {
                System.out.println(err);

            } catch (NullPointerException err) {
                System.out.println(err);

            }

            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            Log.i("ENDTH", ""+state);
            if(state != -1) {
                Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                startActivity(intent);
            }else{
                tvMsg.setText("서버 연결 실패\n ip주소를 다시 확인하여 주세요");
            }

        }
    }



}
