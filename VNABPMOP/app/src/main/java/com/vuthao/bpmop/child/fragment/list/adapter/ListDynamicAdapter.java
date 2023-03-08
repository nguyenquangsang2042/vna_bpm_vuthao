package com.vuthao.bpmop.child.fragment.list.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Filter;
import android.widget.Filterable;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.child.FuncChild;
import com.vuthao.bpmop.home.adapter.HomePageVDTAdapter;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ListDynamicAdapter extends RecyclerView.Adapter implements Filterable {
    private final Activity activity;
    private final ArrayList<DetailList.Headers> headers = new ArrayList<>();
    private ArrayList<JSONObject> objects;
    private final ArrayList<JSONObject> objectsFilter = new ArrayList<>();
    private ListDynamicListener listener;
    private DetailList.Headers headerTitle = null;
    private DetailList.Headers headerIsFollow = null;
    private DetailList.Headers headerFileCount = null;

    private final int VIEW_TYPE_ITEM = 0;
    private final int VIEW_TYPE_LOADING = 1;

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
                    objects.clear();
                    objects.addAll(objectsFilter);
                } else {
                    ArrayList<JSONObject> filteredList = new ArrayList<>();
                    for (JSONObject j : objectsFilter) {
                        if (Functions.removeAccent(j.toString().toLowerCase()).contains(charString)) {
                            filteredList.add(j);
                        }
                    }

                    objects = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface ListDynamicListener {
        void OnDynamicClick(JSONObject object);
    }

    public ListDynamicAdapter(Activity activity, ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> objects, ListDynamicListener listener) {
        this.activity = activity;
        this.objects = objects;
        this.listener = listener;
        objectsFilter.addAll(objects);

        ArrayList<DetailList.Headers> temp = new ArrayList<>(headers);
        for (DetailList.Headers header : temp) {
            switch (header.getInternalName().toLowerCase()) {
                case "title":
                case "content":
                case "tieude": {
                    headerTitle = header;
                    break;
                }
                case "isfollow": {
                    headerIsFollow = header;
                    break;
                }
                case "filecount": {
                    headerFileCount = header;
                    break;
                }
                default: {
                    this.headers.add(header);
                    break;
                }
            }
        }
    }

    public void filter(HashMap<String, String> filters) {
        objects.clear();
        if (filters.isEmpty()) {
            objects.addAll(objectsFilter);
        } else {
            try {
                for (JSONObject object : objectsFilter) {
                    boolean isVisble = false;
                    outer:
                    for (Map.Entry<String, String> entry : filters.entrySet()) {
                        String dateType = entry.getKey().split(";#")[0];
                        String key = entry.getKey().split(";#")[1];

                        switch (dateType) {
                            case VarsControl.SELECTUSERGROUPMULTI:
                            case VarsControl.SELECTUSER: {
                                ArrayList<User> users = new Gson().fromJson(entry.getValue(), new TypeToken<ArrayList<User>>() {}.getType());

                                StringBuilder s = new StringBuilder();
                                if (users != null) {
                                    for (User user : users) {
                                        s.append(user.getID()).append(",");
                                    }
                                }

                                String value = object.get(key).toString();
                                if (entry.getValue().contains(Functions.removeAccent(value.toLowerCase()))) {
                                    isVisble = true;
                                } else {
                                    isVisble = false;
                                    break outer;
                                }

                                break;
                            }
                            case VarsControl.DATETIME: {
                                break;
                            }
                            case VarsControl.SINGLECHOICE: {
                                break;
                            }
                            default: {
                                String value = object.get(key).toString();
                                if (Functions.removeAccent(value.toLowerCase()).contains(entry.getValue())) {
                                    isVisble = true;
                                } else {
                                    isVisble = false;
                                    break outer;
                                }

                                break;
                            }
                        }
                    }

                    if (isVisble) {
                        objects.add(object);
                    }
                }
            } catch (Exception ex) {
                Log.d("ERR filter", ex.getMessage());
            }
        }

        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == VIEW_TYPE_ITEM) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_dynamic_list, parent, false);
            return new ListDynamicHolder(view);
        } else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_load_more, parent, false);
            return new ListDynamicLoadingHolder(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof ListDynamicHolder) {
            populateItemRows((ListDynamicHolder) holder, position);
        } else if (holder instanceof ListDynamicLoadingHolder) {
            showLoadingView((ListDynamicLoadingHolder) holder, position);
        }
    }

    private void showLoadingView(ListDynamicLoadingHolder viewHolder, int position) {
        //ProgressBar would be displayed

    }

    @SuppressLint("SetTextI18n")
    private void populateItemRows(ListDynamicHolder holder, int position) {
        JSONObject object = objects.get(position);
        holder.lnContent.removeAllViews();

        if (position % 2 == 0) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clVer2BlueTint));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
        }

        View rowViewHeader = LayoutInflater.from(holder.itemView.getContext()).inflate(R.layout.item_dynamic_list_item_header, null);
        TextView tvValueHeader = rowViewHeader.findViewById(R.id.tv_ItemDynamicListItem_Header_Value);
        ImageView imgHeaderFavorite = rowViewHeader.findViewById(R.id.img_ItemDynamicListItem_Header_Favorite);
        ImageView imgHeaderFlag = rowViewHeader.findViewById(R.id.img_ItemDynamicListItem_Header_Flag);
        ImageView imgHeaderAttach = rowViewHeader.findViewById(R.id.img_ItemDynamicListItem_Header_AttachFile);

        imgHeaderFlag.setVisibility(View.GONE);

        if (headerTitle != null) {
            tvValueHeader.setText(FuncChild.share.getRawValueByHeader(headerTitle, object));
        } else  {
            tvValueHeader.setText("");
        }

        if (headerIsFollow != null) {
            String value = FuncChild.share.getRawValueByHeader(headerIsFollow, object);
            if (value.equals("1")) {
                imgHeaderFavorite.setImageResource(R.drawable.icon_ver2_star_checked);
            } else {
                imgHeaderFavorite.setImageResource(R.drawable.icon_ver2_star_unchecked);
            }
        } else {
            imgHeaderFavorite.setImageResource(R.drawable.icon_ver2_star_unchecked);
        }

        imgHeaderAttach.setVisibility(View.INVISIBLE);
        if (headerFileCount != null) {
            String value = FuncChild.share.getRawValueByHeader(headerFileCount, object);
            int res = -1;
            if (!Functions.isNullOrEmpty(value)) {
                res =  Integer.parseInt(value);
                if (res > 0) {
                    imgHeaderAttach.setVisibility(View.VISIBLE);
                }
            }
        }

        holder.lnContent.addView(rowViewHeader);

        for (int i = 0; i < headers.size(); i++) {
            View row = LayoutInflater.from(holder.itemView.getContext()).inflate(R.layout.item_dynamic_list_item, null);
            TextView tvTitle = row.findViewById(R.id.tv_ItemDynamicListItem_Title);
            TextView tvValue = row.findViewById(R.id.tv_ItemDynamicListItem_Value);

            String value = FuncChild.share.getFormattedValueByHeader(headers.get(i), object);
            if (value.isEmpty()) {
                continue;
            }

            if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                tvTitle.setText(headers.get(i).getTitle() + ":");
            } else {
                tvTitle.setText(headers.get(i).getTitleEN() + ":");
            }

            tvTitle.setMaxWidth((int)(activity.getResources().getDisplayMetrics().widthPixels * 0.3));
            tvValue.setText(value);

            holder.lnContent.addView(row);
        }
    }

    @Override
    public int getItemCount() {
        return objects.size();
    }

    @Override
    public int getItemViewType(int position) {
        return objects.get(position) == null ? VIEW_TYPE_LOADING : VIEW_TYPE_ITEM;
    }

    public class ListDynamicLoadingHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.prLoadMore)
        ProgressBar prLoadMore;

        public ListDynamicLoadingHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }

    public class ListDynamicHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemDynamicList_All)
        LinearLayout lnAll;
        @BindView(R.id.ln_ItemDynamicList_Content)
        LinearLayout lnContent;

        public ListDynamicHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(view -> {
                listener.OnDynamicClick(objects.get(getAdapterPosition()));
            });
        }
    }
}
