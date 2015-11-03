package com.example.Once;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.transition.TransitionManager;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import java.io.*;
import java.net.Socket;

/**
 * Created by ahndoyoung on 2015. 11. 3..
 */
public class LoginActivity extends Activity {

    public EditText etIP;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);
        initres();

    }

    public void initres(){

        etIP = (EditText) findViewById(R.id.etIP);
        etIP.setText(Preference.getString(this, "ip"));
    }

    public void Connect(View v) {

        try {

            Connector.ip = etIP.getText().toString();
            Connector.getInstance();
            Preference.putString(this, "ip", etIP.getText().toString());
            Intent intent = new Intent(LoginActivity.this, MainActivity.class);
            Toast.makeText(getApplicationContext(), "연결성공", Toast.LENGTH_LONG).show();
            startActivity(intent);

        } catch (IOException err) {
            System.out.println(err);
            Toast.makeText(getApplicationContext(), "연결실패", Toast.LENGTH_LONG).show();
            return;
        } catch (NullPointerException err){
            System.out.println(err);
            Toast.makeText(getApplicationContext(), "IP를 입력해주세요", Toast.LENGTH_LONG).show();
            return;
        }
    }
}
