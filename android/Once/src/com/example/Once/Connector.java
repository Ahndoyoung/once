package com.example.Once;

import java.io.*;
import java.net.Socket;

/**
 * Created by ahndoyoung on 2015. 11. 3..
 */
public class Connector {
    public BufferedReader networkReader = null;
    public BufferedWriter networkWriter = null;
    public PrintWriter printWriter = null;
    public Socket socket = null;
    public int port = 6623;

    public static String ip = null;
    private static Connector instance = null;

    private Connector() throws IOException {

        socket = new Socket(ip, port);
        networkReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
        networkWriter = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
        printWriter = new PrintWriter(networkWriter, true);

    }

    public static Connector getInstance() throws IOException {
        if(instance == null)
            instance = new Connector();
        return instance;
    }

    public static void disConnect() {
        try {
            instance.socket.close();
        } catch (IOException|NullPointerException e) {
            e.printStackTrace();
        }
        instance = null;
        return;
    }
}
