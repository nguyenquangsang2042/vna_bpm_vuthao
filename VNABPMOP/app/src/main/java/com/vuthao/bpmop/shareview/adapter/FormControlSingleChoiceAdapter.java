package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Filter;
import android.widget.Filterable;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.model.custom.LookupData;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class FormControlSingleChoiceAdapter extends RecyclerView.Adapter<FormControlSingleChoiceAdapter.FormControlSingleChoiceHolder> implements Filterable {
    private final Activity _mainAct;
    private ArrayList<LookupData> lstItem;
    private final ArrayList<LookupData> lstItemFilter = new ArrayList<>();
    private final FormControlSingleChoiceListener listener;

    @Override
    public Filter getFilter() {
        return new Filter() {
            @Override
            protected FilterResults performFiltering(CharSequence charSequence) {
                return new FilterResults();
            }

            @Override
            protected void publishResults(CharSequence charSequence, FilterResults filterResults) {
                String charString = Functions.removeAccent(charSequence.toString().toLowerCase());
                if (charString.isEmpty()) {
                    lstItem.clear();
                    lstItem.addAll(lstItemFilter);
                } else {
                    ArrayList<LookupData> filteredList = new ArrayList<>();
                    for (LookupData data : lstItemFilter) {
                        if (Utility.removeAccent(data.getTitle().toLowerCase()).contains(charString)) {
                            filteredList.add(data);
                        }
                    }

                    lstItem = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface FormControlSingleChoiceListener {
        void OnClick(int pos);
    }

    public FormControlSingleChoiceAdapter(Activity _mainAct, Context _context, ArrayList<LookupData> _lstItem, FormControlSingleChoiceListener listener) {
        this._mainAct = _mainAct;
        this.lstItem = _lstItem;
        this.listener = listener;
        lstItemFilter.addAll(_lstItem);
    }

    public void setList(ArrayList<LookupData> datas) {
        lstItemFilter.clear();
        lstItemFilter.addAll(datas);
    }

    @NonNull
    @Override
    public FormControlSingleChoiceHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_form_control_choice, parent,false);
        return new FormControlSingleChoiceHolder(view);
    }

    @Override
    public int getItemCount() {
        return lstItem.size();
    }

    @Override
    public void onBindViewHolder(@NonNull FormControlSingleChoiceHolder holder, int position) {
        LookupData data = lstItem.get(position);
        if (data != null) {
            if (data.isSelected()) {
                holder._imgIsChecked.setVisibility(View.VISIBLE);
                holder._tvTitle.setTextColor(ContextCompat.getColor(_mainAct, R.color.clBottomEnable));
            } else {
                holder._imgIsChecked.setVisibility(View.GONE);
                holder._tvTitle.setTextColor(ContextCompat.getColor(_mainAct, R.color.clBlack));
            }

            if (!Functions.isNullOrEmpty(data.getTitle())) {
                holder._tvTitle.setText(data.getTitle());
            } else {
                holder._tvTitle.setText("");
            }
        }
    }

    public class FormControlSingleChoiceHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemFormControlChoice)
        LinearLayout _lnAll;
        @BindView(R.id.tv_ItemFormControlChoice_Title)
        TextView _tvTitle;
        @BindView(R.id.img_ItemFormControlChoice_Check)
        ImageView _imgIsChecked;

        public FormControlSingleChoiceHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);

            _lnAll.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    listener.OnClick(getAdapterPosition());
                    notifyDataSetChanged();
                }
            });
        }
    }
}
