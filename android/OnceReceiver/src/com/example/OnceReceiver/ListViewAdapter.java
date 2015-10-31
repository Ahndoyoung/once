package com.example.OnceReceiver;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.ArrayList;

/**
 * Created by ahndoyoung on 2015. 10. 31..
 */
public class ListViewAdapter extends BaseAdapter {
    private Context mContext = null;
    private ArrayList<SellingItem> mListData = new ArrayList<SellingItem>();

    public ListViewAdapter(Context mContext) {
        super();
        this.mContext = mContext;

    }

    @Override
    public int getCount() {
        return mListData.size();
    }

    @Override
    public Object getItem(int position) {
        return mListData.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    public void addItem(SellingItem sellingItem){
        mListData.add(sellingItem);
    }
    public void addItem(String content, int quantity, String temp){
        SellingItem addInfo = new SellingItem(content, quantity);
        addInfo.setTemperature(temp);
        mListData.add(addInfo);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        ViewHolder holder;
        if (convertView == null) {
            holder = new ViewHolder();

            LayoutInflater inflater = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.orderlist_item, null);

            holder.tvContent = (TextView) convertView.findViewById(R.id.tvContent);
            holder.tvQuantity = (TextView) convertView.findViewById(R.id.tvQuantity);

            convertView.setTag(holder);
        }else{
            holder = (ViewHolder) convertView.getTag();
        }

        SellingItem mData = mListData.get(position);
        if(mData.getTemperature() !=null) {
            if (mData.getTemperature().equals("I")) {
                holder.tvContent.setBackgroundColor(Color.argb(70, 138, 214, 240));
                holder.tvQuantity.setBackgroundColor(Color.argb(70, 138, 214, 240));
            } else if (mData.getTemperature().equals("H")){
                holder.tvContent.setBackgroundColor(Color.argb(100, 255, 214, 214));
                holder.tvQuantity.setBackgroundColor(Color.argb(100, 255, 214, 214));
            }
        }
        holder.tvContent.setText(" " + mData.getContent());
        holder.tvQuantity.setText(mData.getQuantity() + " ");

        return convertView;
    }
}