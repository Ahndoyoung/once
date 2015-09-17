package com.example.OnceReceiver;

import android.app.Activity;
import android.os.*;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;
import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.Socket;

public class MainActivity extends Activity {

    public Socket socket = null;
    public ChatThread cThread = null;
    public BufferedReader stream = null;

    public TextView tvMsg;
    public Button btnConnect;
    public EditText etIP;

    Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            tvMsg.setText(msg.obj.toString());
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);

        // API9 이상부터는 네트워크 사용 시
        if (Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }

        tvMsg = (TextView) findViewById(R.id.tvMsg);
        btnConnect = (Button) findViewById(R.id.btnConnect);
    }

    public void Connect(View v) {
        try {
            etIP = (EditText) findViewById(R.id.etIP);
            socket = new Socket(etIP.getText().toString(), 9000);
            stream = new BufferedReader(new InputStreamReader(socket.getInputStream()));

            tvMsg.setText("successed");
            btnConnect.setVisibility(btnConnect.INVISIBLE);

            cThread = new ChatThread();
            cThread.start();
        } catch (IOException err) {
            System.out.println(err);
            Toast.makeText(getApplicationContext(), "연결실패", Toast.LENGTH_LONG).show();

            return;
        }
    }

    class ChatThread extends Thread {
        public void run() {
            String sMsg;
            Message msg;

            while (true) {
                try {
                    sMsg = stream.readLine();
                    System.out.println(sMsg);
                    SellingItems sellingItems = new Gson().fromJson(sMsg, SellingItems.class);
                    sMsg = "";
                    msg = new Message();
                    for (SellingItem m : sellingItems.getSellingItems()) {
                        sMsg += m.getContent() + "\n" + m.getQuantity() + "\n";
                    }
                    msg.obj = sMsg;
                    mHandler.sendMessage(msg);
                    System.out.println(sMsg);
                } catch (NullPointerException|IOException err) {
                    System.out.println(err);
                }
            }
        }
    }


}
