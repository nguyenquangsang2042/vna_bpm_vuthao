package com.vuthao.bpmop.child.fragment.list.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.res.ColorStateList;
import android.util.Log;
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

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.child.FuncChild;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import butterknife.BindView;
import butterknife.ButterKnife;
import de.hdodenhof.circleimageview.CircleImageView;
import io.realm.Realm;

public class ListStableAdapter extends RecyclerView.Adapter<ListStableAdapter.ListStableHolder> implements Filterable {
    private final Activity activity;
    private ArrayList<JSONObject> jsonObjects;
    private ArrayList<DetailList.Headers> headers;
    private ArrayList<JSONObject> jsonObjectsFilter = new ArrayList<>();
    private final ListStableListener listener;
    private final Realm realm;

    private DetailList.Headers headerTitle = null;
    private DetailList.Headers headerAssignedTo = null;
    private DetailList.Headers headerCreated = null;
    private DetailList.Headers headerStatus = null;
    private DetailList.Headers headerIsFollow = null;
    private DetailList.Headers headerFileCount = null;
    private DetailList.Headers headerDuedate = null;

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
                    jsonObjects.clear();
                    jsonObjects.addAll(jsonObjectsFilter);
                } else {
                    ArrayList<JSONObject> filteredList = new ArrayList<>();
                    for (JSONObject j : jsonObjectsFilter) {
                        if (j.toString().toLowerCase().contains(charString)) {
                            filteredList.add(j);
                        }
                    }

                    jsonObjects = filteredList;
                }

                notifyDataSetChanged();
            }
        };
    }

    public interface ListStableListener {
        void OnStableClick(JSONObject object);
    }

    public ListStableAdapter(Activity activity, ArrayList<JSONObject> jsonObjects, ArrayList<DetailList.Headers> headers, ListStableListener listener) {
        this.activity = activity;
        this.jsonObjects = jsonObjects;
        this.listener = listener;
        this.headers = headers;
        this.jsonObjectsFilter.addAll(jsonObjects);

        realm = new RealmController().getRealm();

        for (DetailList.Headers header : headers) {
            switch (header.getInternalName().toLowerCase()) {
                case "title":
                case "content": {
                    headerTitle = header;
                    break;
                }
                case "assignedto":
                case "assignedby": {
                    headerAssignedTo = header;
                    break;
                }
                case "created": {
                    headerCreated = header;
                    break;
                }
                case "status": {
                    headerStatus = header;
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
                case "duedate": {
                    headerDuedate = header;
                    break;
                }
            }
        }
    }

    public void filter(HashMap<String, String> filters) {
        jsonObjects.clear();
        if (filters.isEmpty()) {
            jsonObjects.addAll(jsonObjectsFilter);
        } else {
            try {
                for (JSONObject object : jsonObjectsFilter) {
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
                        jsonObjects.add(object);
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
    public ListStableHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_appbase_todo_list, parent, false);
        return new ListStableHolder(view);
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void onBindViewHolder(@NonNull ListStableHolder holder, int position) {
        JSONObject object = jsonObjects.get(position);
        if (position % 2 == 1) {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clWhite));
        } else {
            holder.lnAll.setBackgroundColor(ContextCompat.getColor(activity, R.color.clVer2BlueNavigation));
        }

        holder.lnCategory.setVisibility(View.GONE);

        holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
        if (headerAssignedTo != null) {
            String value = FuncChild.share.getRawValueByHeader(headerAssignedTo, object);
            if (!Functions.isNullOrEmpty(value)) {
                String Id = value.split(",")[0].toLowerCase();
                UserAndGroup userAndGroup = FuncChild.share.getUserAndGroup(Id);
                if (userAndGroup != null) {
                    if (userAndGroup.getType().equals("0")) {
                        ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + userAndGroup.getImagePath(), holder.imgAvatar);
                    } else {
                        holder.imgAvatar.setImageResource(R.drawable.icon_avatar64);
                    }
                }
            }
        }

        if (headerTitle != null) {
            holder.tvTitle.setText(FuncChild.share.getRawValueByHeader(headerTitle, object));
        } else {
            holder.tvTitle.setText("");
        }

        holder.tvTime.setText("");

        if (headerCreated != null) {
            String value = FuncChild.share.getRawValueByHeader(headerCreated, object);
            if (!Functions.isNullOrEmpty(value)) {
                holder.tvTime.setText(Functions.share.formatDateLanguage(value));
            }
        }

        holder.tvDescription.setVisibility(View.INVISIBLE);
        if (headerAssignedTo != null && headerStatus != null) {
            String valueStatus = FuncChild.share.getRawValueByHeader(headerStatus, object);
            String valueAssignedTo = FuncChild.share.getRawValueByHeader(headerAssignedTo, object);

            if (!Functions.isNullOrEmpty(valueStatus) && !Functions.isNullOrEmpty(valueAssignedTo)) {
                String[] lstFullName = Functions.share.getArrayFullNameFromArrayID(valueAssignedTo.split(","));
                if (lstFullName != null && lstFullName.length > 0) {
                    holder.tvDescription.setVisibility(View.VISIBLE);
                    switch (Integer.parseInt(valueStatus)) {
                        case Variable.AppStatusID.Completed:
                            holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_APPROVED", "Phê duyệt: ") + lstFullName[0]);
                            break;
                        case Variable.AppStatusID.Canceled:
                            holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_CANCEL", "Hủy: ") + lstFullName[0]);
                            break;
                        case Variable.AppStatusID.Rejected:
                            holder.tvDescription.setText(Functions.share.getTitle("TEXT_TITLE_REJECTED", "Từ chối: ") + lstFullName[0]);
                            break;
                        default:
                            Functions.share.setTextView_FormatMultiUser(holder.tvDescription, lstFullName, false, true);
                            break;
                    }
                }
            }
        }

        holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_unchecked);
        if (headerIsFollow != null) {
            String value = FuncChild.share.getRawValueByHeader(headerIsFollow, object);
            if (value.equals("1")) {
                holder.imgFavorite.setImageResource(R.drawable.icon_ver2_star_checked);
            }
        }

        holder.imgAttachFile.setVisibility(View.INVISIBLE);
        if (headerFileCount != null) {
            String value = FuncChild.share.getRawValueByHeader(headerFileCount, object);
            int res = -1;
            if (!Functions.isNullOrEmpty(value)) {
                res = Integer.parseInt(value);
                if (res > 0) {
                    holder.imgAttachFile.setVisibility(View.VISIBLE);
                }
            }
        }

        holder.imgFlag.setVisibility(View.GONE);

        holder.tvStatus.setVisibility(View.INVISIBLE);
        holder.tvStatusTime.setText("");

        if (headerStatus != null) {
            String value = FuncChild.share.getRawValueByHeader(headerStatus, object);
            if (!Functions.isNullOrEmpty(value)) {
                AppStatus appStatus = realm.where(AppStatus.class)
                        .equalTo("ID", Integer.parseInt(value))
                        .findFirst();
                if (appStatus != null) {
                    holder.tvStatus.setVisibility(View.VISIBLE);
                    if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
                        holder.tvStatus.setText(appStatus.getTitle());
                    } else {
                        holder.tvStatus.setText(appStatus.getTitleEN());
                    }
                    holder.tvStatus.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByAppStatus(appStatus.getID())));

                    if (headerDuedate != null) {
                        value = FuncChild.share.getRawValueByHeader(headerDuedate, object);
                        switch (appStatus.getID()) {
                            case Variable.AppStatusID.Completed:
                            case Variable.AppStatusID.Canceled:
                            case Variable.AppStatusID.Rejected: {
                                holder.tvStatusTime.setText("");
                                break;
                            }
                            default: {
                                holder.tvStatusTime.setTextColor(Functions.share.getColorByDueDate(value));
                                if (Functions.isNullOrEmpty(value)) {
                                    holder.tvStatusTime.setText("");
                                } else {
                                    holder.tvStatusTime.setText(Functions.share.getDateLanguage(value, 1, CurrentUser.getInstance().getUser().getLanguage()));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    @Override
    public int getItemCount() {
        return jsonObjects.size();
    }

    public class ListStableHolder extends RecyclerView.ViewHolder {
        @BindView(R.id.ln_ItemAppBaseToDoList_All)
        LinearLayout lnAll;
        @BindView(R.id.ln_ItemAppBaseToDoList_Category)
        LinearLayout lnCategory;
        @BindView(R.id.tv_ItemAppBaseToDoList_Category)
        TextView tvCategory;
        @BindView(R.id.img_ItemAppBaseToDoList_Avatar)
        CircleImageView imgAvatar;
        @BindView(R.id.tv_ItemAppBaseToDoList_Title)
        TextView tvTitle;
        @BindView(R.id.tv_ItemAppBaseToDoList_Time)
        TextView tvTime;
        @BindView(R.id.tv_ItemAppBaseToDoList_Description)
        TextView tvDescription;
        @BindView(R.id.img_ItemAppBaseToDoList_Favorite)
        ImageView imgFavorite;
        @BindView(R.id.img_ItemAppBaseToDoList_Flag)
        ImageView imgFlag;
        @BindView(R.id.img_ItemAppBaseToDoList_AttachFile)
        ImageView imgAttachFile;
        @BindView(R.id.tv_ItemAppBaseToDoList_Status)
        TextView tvStatus;
        @BindView(R.id.tv_ItemAppBaseToDoList_StatusTime)
        TextView tvStatusTime;

        public ListStableHolder(@NonNull View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
            lnAll.setOnClickListener(view -> {
                listener.OnStableClick(jsonObjects.get(getAdapterPosition()));
            });
        }
    }
}
