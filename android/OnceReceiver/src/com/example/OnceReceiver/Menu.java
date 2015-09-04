package com.example.OnceReceiver;
import java.lang.String;

/**
 * Created by ahndoyoung on 2015. 9. 3..
 */
public class Menu {

    private String content;
    private int quantity;

    /*
        getter setter
     */
    public int getQuantity() {
        return quantity;
    }

    public void setQuantity(int quantity) {
        this.quantity = quantity;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }


    public Menu(String content, int quantity) {

        this.content = content;
        this.quantity = quantity;

    }

    public Menu(){
        content = null;
    }
}
