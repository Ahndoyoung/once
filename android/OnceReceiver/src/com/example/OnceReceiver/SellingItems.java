package com.example.OnceReceiver;

import java.util.ArrayList;

/**
 * Created by ahndoyoung on 2015. 9. 8..
 */
public class SellingItems {

    private ArrayList<SellingItem> sellingItems;

    private int id;

    public SellingItems(ArrayList<SellingItem> sellingItems, int id) {
        this.sellingItems = sellingItems;
        this.id = id;
    }

    public ArrayList<SellingItem> getSellingItems() {
        return sellingItems;
    }

    public void setSellingItems(ArrayList sellingItems) {
        this.sellingItems = sellingItems;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

}
