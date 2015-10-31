package com.example.OnceReceiver;

import android.app.Activity;
import android.content.Context;
import android.os.*;
import android.transition.Scene;
import android.transition.Transition;
import android.transition.TransitionManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.*;
import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.ConnectException;
import java.net.Socket;

public class MainActivity extends Activity {

    public Socket socket = null;
    public ChatThread cThread = null;
    public BufferedReader stream = null;

    public TextView tvMsg;
    public Button btnConnect;
    public EditText etIP;

    ViewGroup rootContainer;
    Scene loginScene;
    Scene viewScene;
    LinearLayout counter;

    int num = 0;


    Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            // TODO display

        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.main);
        rootContainer = (ViewGroup) findViewById(R.id.rootContainer);
        counter = (LinearLayout) findViewById(R.id.counterlayout);
        loginScene = Scene.getSceneForLayout(rootContainer, R.layout.login, this);
        viewScene = Scene.getSceneForLayout(rootContainer, R.layout.view, this);


        loginScene.enter();


        // API9 이상부터는 네트워크 사용 시
        if (Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }

        tvMsg = (TextView) findViewById(R.id.tvMsg);
        btnConnect = (Button) findViewById(R.id.btnConnect);
    }

    public void Connect(View v) {
                    TransitionManager.go(viewScene);

//        try {
//            etIP = (EditText) findViewById(R.id.etIP);
//            String ip = etIP.getText().toString();
//            Log.i("tcp ", "ip: " + ip);
//            socket = new Socket(ip, 9051);
//            stream = new BufferedReader(new InputStreamReader(socket.getInputStream()));
//
//
//            Toast.makeText(getApplicationContext(), "연결성공", Toast.LENGTH_LONG).show();
//            TransitionManager.go(viewScene);
//            cThread = new ChatThread();
//            cThread.start();
//
//        } catch (IOException err) {
//            System.out.println(err);
//            Toast.makeText(getApplicationContext(), "연결실패", Toast.LENGTH_LONG).show();
//            return;
//        } catch (NullPointerException err){
//            System.out.println(err);
//            Toast.makeText(getApplicationContext(), "IP를 입력해주세요", Toast.LENGTH_LONG).show();
//            return;
//        }
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
                    break;
                }
            }
            msg = new Message();
            msg.obj="connection fail";
            mHandler.sendMessage(msg);
        }
    }

    public void addOrder(View arg){
        Log.i("add", "order: " + num);
        View v = getLayoutInflater().inflate(R.layout.order, null);
        ((TextView)v.findViewById(R.id.tvOrderNum)).setText("Order #"+num);
        RelativeLayout.LayoutParams parm = new RelativeLayout.LayoutParams(750, ViewGroup.LayoutParams.MATCH_PARENT);
        parm.setMargins(20,0,20,0);
        v.setLayoutParams(parm);
        counter = (LinearLayout) findViewById(R.id.counterlayout);
        counter.addView(v, 0);
        num ++;
        num++;

    }


}
