package com.vuthao.bpmop.core.component;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.graphics.Typeface;
import android.text.TextUtils;
import android.util.Log;
import android.util.Pair;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.custom.CustomEnabledHorizontalScrollView;
import com.vuthao.bpmop.base.model.custom.LookupData;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.model.custom.WFDetailsHeader;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsControl;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.adapter.ListGridDataAdapter;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigInteger;
import java.util.ArrayList;
import java.util.stream.Collectors;

import butterknife.BindView;
import butterknife.ButterKnife;

public class ControlInputGridDetails extends ControlBase {
    private Context context;
    private final LinearLayout parentView;
    private LinearLayout lnTitleImport; // chứa _tvTitle và _lnImport
    private LinearLayout lnImport;
    private CardView cardViewDetail;
    private ImageView imgImport;
    private TextView tvImport;
    private RecyclerView recyListGridData; // Recy to nhất
    private CustomEnabledHorizontalScrollView scrollListGridData;

    private final ViewElement element;
    private ArrayList<WFDetailsHeader> lstHeader = new ArrayList<>();
    private final ArrayList<JSONObject> lstJObjectRow = new ArrayList<>();

    private final int flagView;

    public ControlInputGridDetails(Activity mainAct, LinearLayout parentView, ViewElement element, int flagView) {
        super(mainAct);
        this.parentView = parentView;
        this.element = element;
        this.flagView = flagView;
        initializeComponent();
    }

    @Override
    public void initializeComponent() {
        super.initializeComponent();

        recyListGridData = new RecyclerView(mainAct);
        scrollListGridData = new CustomEnabledHorizontalScrollView(mainAct);

        cardViewDetail = new CardView(mainAct);
        lnTitleImport = new LinearLayout(mainAct);
        lnImport = new LinearLayout(mainAct);
        tvImport = new TextView(mainAct);
        imgImport = new ImageView(mainAct);

        lnTitleImport.setOrientation(LinearLayout.HORIZONTAL);
        lnTitleImport.setGravity(Gravity.CENTER);

        lnImport.setOrientation(LinearLayout.HORIZONTAL);
        lnImport.setGravity(Gravity.RIGHT);

        tvTitle.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.BOLD);
        tvTitle.setTextColor(ContextCompat.getColor(mainAct, R.color.clBlack));
        tvTitle.setMaxLines(1);
        tvTitle.setEllipsize(TextUtils.TruncateAt.END);

        tvImport.setTextSize(TypedValue.COMPLEX_UNIT_SP, 12);
        tvImport.setTypeface(ResourcesCompat.getFont(mainAct, R.font.fontarial), Typeface.NORMAL);
        tvImport.setTextColor(ContextCompat.getColor(mainAct, R.color.clBottomDisable));
        tvImport.setEllipsize(TextUtils.TruncateAt.END);
        tvImport.setGravity(Gravity.CENTER);

        imgImport.setColorFilter(ContextCompat.getColor(mainAct, R.color.clBottomEnable));

        cardViewDetail.setBackgroundTintList(ColorStateList.valueOf(ContextCompat.getColor(mainAct, R.color.clGray)));
        cardViewDetail.setUseCompatPadding(true);
        cardViewDetail.setRadius(5f);

        scrollListGridData.setScrollbarFadingEnabled(false);
        scrollListGridData.setEnableScrolling(true);
        tvImport.setText(Functions.share.getTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới"));
    }

    @Override
    public void initializeFrameView(LinearLayout frame) {
        if (element.isHidden()) return;
        context = frame.getContext();
        super.initializeFrameView(frame);

        tvValue.setVisibility(View.GONE);
        lnContent.removeView(tvTitle);  // Remove ra lát Add lại

        int padding = (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 6, mainAct.getResources().getDisplayMetrics());
        LinearLayout.LayoutParams paramsRecyListGridData = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsCardView = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramslnTitleImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1);
        LinearLayout.LayoutParams paramsTitle = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WRAP_CONTENT, 1);
        LinearLayout.LayoutParams paramslnImport = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
        LinearLayout.LayoutParams paramsimgImport = new LinearLayout.LayoutParams(Functions.share.convertDpToPixel(20, frame.getContext()), Functions.share.convertDpToPixel(20, frame.getContext()));

        paramsCardView.setMargins(padding, 0, padding, padding);
        paramsimgImport.setMargins(padding, 0, 2 * padding, 0);
        paramslnTitleImport.setMargins(0, 0, 0, padding);

        imgImport.setBackground(ContextCompat.getDrawable(frame.getContext(), R.drawable.icon_ver2_addfile));

        lnTitleImport.setLayoutParams(paramslnTitleImport);
        tvTitle.setLayoutParams(paramsTitle);
        lnImport.setLayoutParams(paramslnImport);
        imgImport.setLayoutParams(paramsimgImport);
        cardViewDetail.setLayoutParams(paramsCardView);
        scrollListGridData.setLayoutParams(paramsRecyListGridData);

        lnTitleImport.setPadding(padding, 0, padding, 0);
        imgImport.setPadding(padding, 0, padding, 0);
        lnImport.setPadding(padding, 0, 2 * padding, 0);

        lnImport.addView(imgImport);
        lnImport.addView(tvImport);
        lnTitleImport.addView(tvTitle);
        lnTitleImport.addView(lnImport);

        scrollListGridData.addView(recyListGridData);

        // enable mới cho click
        if (element.isEnable()) {
            lnImport.setVisibility(View.VISIBLE);
            lnImport.setOnClickListener(v -> {
                Intent intent = new Intent();
                intent.setAction(VarsReceiver.INNERACTIONCLICK);
                intent.putExtra("element", new Gson().toJson(element));
                intent.putExtra("actionId", Vars.ControlInputGridDetails_InnerActionID.Create);
                intent.putExtra("positionToAction", -1);
                intent.putExtra("flagViewID", flagView);
                BroadcastUtility.send(mainAct, intent);
            });
        } else {
            lnImport.setVisibility(View.INVISIBLE);
        }

        cardViewDetail.addView(scrollListGridData);
        frame.addView(lnTitleImport);
        frame.addView(cardViewDetail);
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void setTitle() {
        super.setTitle();

        tvTitle.setText(element.getTitle());
        if (element.isRequire() && element.isEnable()) {
            tvTitle.setText(tvTitle.getText() + " (*)");
            Functions.share.setTVHighlightControl(mainAct, tvTitle);
        }
    }

    @Override
    public void setValue() {
        super.setValue();

        if (!Functions.isNullOrEmpty(element.getDataSource())) {
            lstHeader = new Gson().fromJson(element.getDataSource(), new TypeToken<ArrayList<WFDetailsHeader>>() {
            }.getType());

            lstHeader = (ArrayList<WFDetailsHeader>) lstHeader.stream().filter(r -> !Functions.isNullOrEmpty(r.getInternalName())).collect(Collectors.toList());
        }

        if (!Functions.isNullOrEmpty(element.getValue())) {
            try {
                JSONArray json = new JSONArray(element.getValue());
                for (int i = 0; i < json.length(); i++) {
                    lstJObjectRow.add(json.getJSONObject(i));
                }
            } catch (JSONException e) {
                Log.d("ERR Convert JSON", e.getMessage());
            }
        }

        ListGridDataAdapter adapterListGridData = new ListGridDataAdapter(mainAct, context, lstHeader, lstJObjectRow, parentView, element, flagView);
        recyListGridData.setAdapter(adapterListGridData);
        recyListGridData.setLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false));
        recyListGridData.setNestedScrollingEnabled(true);
        recyListGridData.setHorizontalFadingEdgeEnabled(true);
    }
}
