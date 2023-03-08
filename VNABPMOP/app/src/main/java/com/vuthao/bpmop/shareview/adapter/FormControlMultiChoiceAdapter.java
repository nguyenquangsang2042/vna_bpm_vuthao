package com.vuthao.bpmop.shareview.adapter;

import android.app.Activity;
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
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;

public class FormControlMultiChoiceAdapter extends RecyclerView.Adapter<FormControlMultiChoiceAdapter.FormControlMultiChoiceHolder> implements Filterable {
    private Activity mainAct;
    private ArrayList<LookupData> lstItem;
    private ArrayList<LookupData> lstItemFilter = new ArrayList<>();
    private FormControlMultiChoiceListener listener;

    public FormControlMultiChoiceAdapter(Activity mainAct, ArrayList<LookupData> lstItem, FormControlMultiChoiceListener listener) {
        this.mainAct = mainAct;
        this.lstItem = lstItem;
        this.listener = listener;
        lstItemFilter.addAll(lstItem);
    }

    public void setList(ArrayList<LookupData> datas) {
        lstItemFilter.clear();
        lstItemFilter.addAll(datas);
    }

    public ArrayList<LookupData> getListIsClicked() {
        lstItem = (ArrayList<LookupData>) lstItem.stream().filter(LookupData::isSelected).collect(Collectors.toList());
        return lstItem;
    }
    @NonNull
    @Override
    public FormControlMultiChoiceHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_form_control_mutil_choice, parent, false);
        return new FormControlMultiChoiceHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull FormControlMultiChoiceAdapter.FormControlMultiChoiceHolder holder, int position) {
        LookupData lookup = lstItem.get(position);
        if (lookup != null) {
            if (lookup.isSelected()) {
                holder.imgIsChecked.setVisibility(View.VISIBLE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomEnable));
            } else {
                holder.imgIsChecked.setVisibility(View.GONE);
                holder.tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
            }

            if (!Functions.isNullOrEmpty(lookup.getTitle())) {
                holder.tvTitle.setText(lookup.getTitle());
            } else {
                holder.tvTitle.setText("");
            }
        }
    }

    @Override
    public int getItemCount() {
        return lstItem.size();
    }

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
                    lstItem = lstItemFilter;
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

    public interface FormControlMultiChoiceListener {
        void OnClick(LookupData data);
    }



    public class FormControlMultiChoiceHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemFormControlMultiChoice)
        LinearLayout lnAll;
        @BindView(R.id.tv_ItemFormControlMultiChoice_Title)
        TextView tvTitle;
        @BindView(R.id.img_ItemFormControlMultiChoice_Check)
        ImageView imgIsChecked;

        public FormControlMultiChoiceHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(v -> listener.OnClick(lstItem.get(getAdapterPosition())));
        }
    }
}
