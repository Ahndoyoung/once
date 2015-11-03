package com.example.Once;

import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.media.Ringtone;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.*;
import android.transition.Scene;
import android.transition.TransitionManager;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.*;
import com.google.gson.Gson;

import java.io.*;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

public class MainActivity extends Activity {

    public TextView tvMsg;
    public Button btnConnect;
    public EditText etIP;

    ArrayList<SellingItems> orderList;
    Map<Integer, View> viewTable;
    ViewGroup rootContainer;
    LinearLayout counter;

    String confirm_msg;

    Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            if(msg.what == 1){
                SellingItems val = (SellingItems) msg.obj;
                orderList.add(val);
                addOrder(val, false);
            }else if(msg.what == 2){
                DeleteOrder val = (DeleteOrder) msg.obj;
                int num = val.getId();
                View v = viewTable.get(num);
                counter = (LinearLayout) findViewById(R.id.counterlayout);
                counter.removeView(v);
            }else if(msg.what == 3){
                SellingItems val = (SellingItems) msg.obj;
                orderList.add(val);
                addOrder(val, true);
            }else if(msg.what == -1){
                Intent intent = new Intent(MainActivity.this, LoginActivity.class);
                startActivity(intent);
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.view);
        rootContainer = (ViewGroup) findViewById(R.id.rootContainer);
        counter = (LinearLayout) findViewById(R.id.counterlayout);

        orderList = new ArrayList<>();
        viewTable = new HashMap<>();

        // API9 이상부터는 네트워크 사용 시
        if (Build.VERSION.SDK_INT > 9) {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
            StrictMode.setThreadPolicy(policy);
        }

        tvMsg = (TextView) findViewById(R.id.tvMsg);
        btnConnect = (Button) findViewById(R.id.btnConnect);

    }

    @Override
    protected void onResume() {
        super.onResume();
        try {

            Connector.getInstance();
            new ChatThread().execute(null, null, null);

        } catch (IOException|NullPointerException e) {
            e.printStackTrace();
            finish();
        }
        Log.i("onResume", "");
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        try {
            Connector.getInstance().socket.close();
            Connector.disConnect();
        } catch (IOException|NullPointerException e) {
            e.printStackTrace();
        }
        Log.i("onDestory", "");
    }

    class ChatThread extends AsyncTask<Void, Void, Void> {
        @Override
        protected Void doInBackground(Void... params) {
            String sMsg;
            Message msg;

            while (true) {
                try {
                    sMsg = Connector.getInstance().networkReader.readLine();
                    System.out.println(sMsg);
                    msg = new Message();

                    System.out.println(sMsg);

                    System.out.println("method" + sMsg.charAt(0));
                    System.out.println("method" + sMsg.charAt(1));

                    if(sMsg.charAt(1) == '1'){
                        sMsg = sMsg.substring(2,sMsg.length());
                        SellingItems sellingItems = new Gson().fromJson(sMsg, SellingItems.class);
                        msg.what = 1;
                        msg.obj = sellingItems;
                    }else if(sMsg.charAt(1) == '2'){
                        sMsg = sMsg.substring(2,sMsg.length());
                        DeleteOrder deleteOrder = new Gson().fromJson(sMsg, DeleteOrder.class);
                        msg.what = 2;
                        msg.obj = deleteOrder;
                    }else if(sMsg.charAt(1) == '3'){
                        sMsg = sMsg.substring(2,sMsg.length());
                        SellingItems sellingItems = new Gson().fromJson(sMsg, SellingItems.class);
                        msg.what = 3;
                        msg.obj = sellingItems;
                    }

                    mHandler.sendMessage(msg);
                } catch (NullPointerException|IOException err) {
                    System.out.println(err);
                    break;
                }
            }
            msg = new Message();
            msg.what = -1;
            mHandler.sendMessage(msg);
            return null;
        }

    }

    public void addOrder(SellingItems arg, Boolean isModified){
        int ordernum = arg.getId();

        View v = getLayoutInflater().inflate(R.layout.order, null);
        if(!isModified) {
            ((TextView) v.findViewById(R.id.tvOrderNum)).setText("Order #" + ordernum);
        }else{
            ((TextView) v.findViewById(R.id.tvOrderNum)).setText("Order #" + ordernum + " (M)");
        }
        ((Button)v.findViewById(R.id.btnConfirm)).setText(ordernum+ "# " +"완료");
        ListView orderListView = (ListView)v.findViewById(R.id.listOrder);
        ListViewAdapter orderListViewAdpater = new ListViewAdapter(v.getContext());
        orderListView.setAdapter(orderListViewAdpater);


        for (SellingItem m : arg.getSellingItems()) {
            orderListViewAdpater.addItem(m);
        }

        RelativeLayout.LayoutParams parm = new RelativeLayout.LayoutParams(750, ViewGroup.LayoutParams.MATCH_PARENT);
        v.setLayoutParams(parm);
        v.setPadding(10,0,10,0);


        counter = (LinearLayout) findViewById(R.id.counterlayout);
        counter.addView(v, 0);

        viewTable.put(ordernum, v);


    }

    public void confirmOrder(View arg){

        int ordernum = Integer.parseInt(((Button)arg).getText().toString().split("#")[0]);
        Log.i("confirm: ", ""+ordernum);
        View v = viewTable.get(ordernum);
        counter = (LinearLayout) findViewById(R.id.counterlayout);
        counter.removeView(v);
        viewTable.remove(ordernum);

        confirm_msg = "" + 2 +"{\"id\" : " + ordernum + "}";

        new SendThread().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, confirm_msg);

    }

    class SendThread extends AsyncTask<String, Void, Void>{
        String sendmsg = null;
        @Override
        protected Void doInBackground(String... msg) {
            try {
                sendmsg = msg[0];
                Connector.getInstance().printWriter.println(sendmsg);

            } catch (IOException|NullPointerException e) {
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onPostExecute(Void aVoid) {
            Log.i("send", sendmsg);
        }
    }

    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
    private void StartAsyncTaskInParallel(SendThread task) {
        if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB)
            task.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
        else
            task.execute();
    }
}
