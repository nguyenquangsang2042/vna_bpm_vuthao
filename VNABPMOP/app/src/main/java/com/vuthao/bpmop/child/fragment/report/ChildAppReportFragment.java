package com.vuthao.bpmop.child.fragment.report;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Typeface;
import android.os.AsyncTask;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.os.Handler;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.github.mikephil.charting.animation.Easing;
import com.github.mikephil.charting.charts.BarChart;
import com.github.mikephil.charting.charts.PieChart;
import com.github.mikephil.charting.components.Legend;
import com.github.mikephil.charting.data.BarData;
import com.github.mikephil.charting.data.PieData;
import com.github.mikephil.charting.data.PieDataSet;
import com.github.mikephil.charting.data.PieEntry;
import com.github.mikephil.charting.formatter.DefaultValueFormatter;
import com.github.mikephil.charting.interfaces.datasets.IBarDataSet;
import com.github.mikephil.charting.utils.ColorTemplate;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.custom.DetailList;
import com.vuthao.bpmop.base.model.custom.ObjectPropertySearch;
import com.vuthao.bpmop.base.model.custom.UserAndGroup;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.child.fragment.list.adapter.ListDynamicAdapter;
import com.vuthao.bpmop.child.fragment.report.presenter.ChildAppReportPresenter;
import com.vuthao.bpmop.shareview.ShareView_PopupFilterList;

import org.json.JSONObject;
import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class ChildAppReportFragment extends BaseFragment implements TextWatcher, ShareView_PopupFilterList.PopupFilterListListener, ListDynamicAdapter.ListDynamicListener, ChildAppReportPresenter.ChildAppReportListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.img_ViewReport_Back)
    ImageView imgBack;
    @BindView(R.id.tv_ViewReport_Title)
    TextView tvTitle;
    @BindView(R.id.tv_ViewReport_SubTitle)
    TextView tvSubTitle;
    @BindView(R.id.swipe_ViewList)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewReport_Toolbar)
    LinearLayout lnToolbar;
    @BindView(R.id.img_ViewReport_Filter)
    ImageView imgFilter;
    @BindView(R.id.img_ViewReport_SubTitle_Previous)
    ImageView imgSubPrevious;
    @BindView(R.id.img_ViewReport_SubTitle_Next)
    ImageView imgSubNext;
    @BindView(R.id.tv_ViewReport_NoData2)
    TextView tvNoData2;
    @BindView(R.id.ln_ViewReport_Search)
    LinearLayout lnSearch;
    @BindView(R.id.edt_ViewReport_Search)
    EditText edtSearch;
    @BindView(R.id.img_ViewReport_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.img_ViewReport_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.recy_ViewReport_Category_Dynamic)
    RecyclerView recyCategoryDynamic;
    @BindView(R.id.ln_ViewReport_Category_Dynamic)
    LinearLayout lnCategoryDynamic;
    @BindView(R.id.ln_ViewReport_NoData2)
    LinearLayout lnNoData2;
    @BindView(R.id.ln_ViewReport_lnLoading)
    LinearLayout lnLoading;
    @BindView(R.id.rlView)
    RelativeLayout rlView;
    @BindView(R.id.ln_ViewReport_Chart)
    LinearLayout lnChart;
    @BindView(R.id.web_ViewReport_Chart)
    WebView web_ViewReport_Chart;

    private View rootView;
    private Workflow workflow;
    private ArrayList<ResourceView> lstResourceView;
    ArrayList<JSONObject> lstJObjectDynamic;
    private ArrayList<DetailList.Headers> headers;
    private ArrayList<DetailList.Headers> columns;
    private ArrayList<JSONObject> workflowCharts;
    private ResourceView currentResourceView;
    private ChildAppReportPresenter presenter;
    private ListDynamicAdapter adapter;
    private AnimationController animationController;
    private ShareView_PopupFilterList popupFilterList;
    private ObjectPropertySearch objectPropertySearch;
    private int position = 0;
    private boolean isLoading = false;
    private LinearLayoutManager mLayoutManager;
    private BarChart barChart;
    private PieChart pieChart;
    private float defaultBarWidth = -1;

    public ChildAppReportFragment(Workflow workflow) {
        this.workflow = workflow;
    }

    public ChildAppReportFragment() {
        // Required empty public constructor
    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_child_app_report, container, false);
            ButterKnife.bind(this, rootView);

            init();
            configWebView();
            setTitle();
            setData();
            initScrollListener();

            swipe.setEnabled(false);
            swipe.setOnRefreshListener(this);
            edtSearch.addTextChangedListener(this);
        }
        return rootView;
    }

    private void init() {
        presenter = new ChildAppReportPresenter(this);
        animationController = new AnimationController();
        lstResourceView = new ArrayList<>();
        lstJObjectDynamic = new ArrayList<>();
        headers = new ArrayList<>();
        columns = new ArrayList<>();
        workflowCharts = new ArrayList<>();
        popupFilterList = new ShareView_PopupFilterList(requireActivity(), rlView, this);

        Utility.share.setupSwipeRefreshLayout(swipe);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.FORMCLICK);
        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.UPDATEFORM);

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        recyCategoryDynamic.setLayoutManager(mLayoutManager);
        recyCategoryDynamic.setDrawingCacheEnabled(true);
        recyCategoryDynamic.setHasFixedSize(false);
        recyCategoryDynamic.setItemViewCacheSize(20);
    }

    private void configWebView() {
        WebSettings webSettings = web_ViewReport_Chart.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setBuiltInZoomControls(true);
        web_ViewReport_Chart.setVerticalScrollBarEnabled(true);
        web_ViewReport_Chart.setHorizontalScrollBarEnabled(true);
        webSettings.setSupportZoom(true);
        webSettings.setBuiltInZoomControls(true);
        webSettings.setDisplayZoomControls(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setUseWideViewPort(true);
        webSettings.setUserAgentString("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
        //web_ViewReport_Chart.setWebViewClient(new MyWebViewClient());

    }
    private void setTitle() {
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvTitle.setText(workflow.getTitle());
        } else {
            tvTitle.setText(workflow.getTitleEN());
        }

        tvNoData2.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
    }

    private void setData() {
        lstResourceView = presenter.getReports(workflow.getWorkflowID());
        if (!lstResourceView.isEmpty()) {
            setResourceView(lstResourceView.get(0));
            position = 0;
        } else {
            lnNoData2.setVisibility(View.VISIBLE);
            lnCategoryDynamic.setVisibility(View.GONE);
            lnLoading.setVisibility(View.GONE);
        }
    }

    private void initScrollListener() {
        recyCategoryDynamic.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (isLoading) {
                    if (mLayoutManager.findLastCompletelyVisibleItemPosition() == lstJObjectDynamic.size() - 1) {
                        loadMore();
                        isLoading = false;
                    }
                }
            }
        });
    }

    private void loadMore() {
        lstJObjectDynamic.add(null);
        adapter.notifyItemInserted(lstJObjectDynamic.size() - 1);

        objectPropertySearch.setOffset(lstJObjectDynamic.size() - 1);
        Handler handler = new Handler();

        handler.postDelayed(() -> {
            presenter.getDynamicMoreFormField(objectPropertySearch);
        }, 1000);
    }

    private void setResourceView(ResourceView resourceView) {
        currentResourceView = resourceView;
        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvSubTitle.setText(currentResourceView.getTitle());
        } else {
            tvSubTitle.setText(currentResourceView.getTitleEN());
        }

        presenter.getChartColunms(currentResourceView.getID());
        presenter.getDynamicFormField(currentResourceView.getID());
    }

    private void setListDynamic() {
        lnNoData2.setVisibility(View.GONE);
        lnCategoryDynamic.setVisibility(View.VISIBLE);
        //lnChart.setVisibility(View.VISIBLE);

        adapter = new ListDynamicAdapter(requireActivity(), headers, lstJObjectDynamic, this);
        recyCategoryDynamic.setAdapter(adapter);
    }

    private void previous() {
        int index = position - 1 >= 0 ? position - 1 : position;
        if (index != -1 && index != position) {
            lnLoading.setVisibility(View.VISIBLE);
            position = index;
            setResourceView(lstResourceView.get(index));
            if (!edtSearch.getText().toString().isEmpty()) {
                edtSearch.setText("");
            }
        }
    }

    private void next() {
        int index = position + 1 <= lstResourceView.size() - 1 ? position + 1 : position;
        if (index != -1 && index != position) {
            lnLoading.setVisibility(View.VISIBLE);
            position = index;
            setResourceView(lstResourceView.get(index));
            if (!edtSearch.getText().toString().isEmpty()) {
                edtSearch.setText("");
            }
        }
    }

    private void filter() {
        popupFilterList.filter(headers, currentResourceView, objectPropertySearch);
    }

    private void search() {
        if (lnSearch.getVisibility() == View.VISIBLE) {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
            lnSearch.setVisibility(View.GONE);
            edtSearch.setText("");
            KeyboardManager.hideKeyboard(edtSearch, requireActivity());
        } else {
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(edtSearch, requireActivity());
        }
    }

    private void initBarChart() {
        try {
            lnChart.setVisibility(View.GONE);
            web_ViewReport_Chart.setVisibility(View.VISIBLE);
            Map<String, String> additionalHttpHeaders = new HashMap<>();
            additionalHttpHeaders.put("Cookie", BaseActivity.getToken());
            additionalHttpHeaders.put("Set-Cookie", BaseActivity.getToken());
            web_ViewReport_Chart.setWebViewClient(new WebViewClient() {
                @Override
                public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                    view.loadUrl(request.getUrl().toString(), additionalHttpHeaders);
                    return false;
                }
            });
            web_ViewReport_Chart.loadUrl("https://bpm.vuthao.com/workflow/SitePages/ReportViewForm.aspx?wid=1142&RID=2721", additionalHttpHeaders);

           /* HashMap<String, List<BarEntry>> hashMap = new HashMap<>();
            for (int i = 0; i < workflowCharts.size(); i++) {
                String description = presenter.getDescription(columns.get(1), workflowCharts.get(i));
                String value = presenter.getRawValue(columns.get(1), workflowCharts.get(i));
                // User/Group
                if (columns.get(1).getFieldTypeId() == 7) {
                    List<BarEntry> barEntries = new ArrayList<>();
                    String firstChar = description.split(",")[0];
                    UserAndGroup userAndGroup = presenter.getUserChart(firstChar);
                    String title = Functions.isNullOrEmpty(userAndGroup.getName()) ? "" : userAndGroup.getName();
                    if (hashMap.containsKey(title)) {
                        barEntries = hashMap.get(title);
                    }

                    barEntries.add(new BarEntry(i + 1, value.isEmpty() ? 0 : Float.parseFloat(value)));
                    hashMap.put(title, barEntries);
                } else if (columns.get(1).getInternalName().toLowerCase().equals("status")) {
                    List<BarEntry> barEntries = new ArrayList<>();
                    AppStatus status = presenter.getStatus(description);
                    if (hashMap.containsKey(description)) {
                        barEntries = hashMap.get(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? status.getTitle() : status.getTitleEN());
                    }

                    barEntries.add(new BarEntry(i + 1, value.isEmpty() ? 0 : Float.parseFloat(value)));
                    hashMap.put(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? status.getTitle() : status.getTitleEN(), barEntries);
                } else {
                    List<BarEntry> barEntries = new ArrayList<>();
                    if (hashMap.containsKey(description)) {
                        barEntries = hashMap.get(description);
                    }

                    barEntries.add(new BarEntry(i + 1, value.isEmpty() ? 0 : Float.parseFloat(value)));
                    hashMap.put(description, barEntries);
                }
            }*/

        /*// sort List by value size desecsing
        hashMap = hashMap.entrySet()
                .stream()
                .sorted(comparingInt(e -> -e.getValue().size()))
                .collect(toMap(
                        Map.Entry::getKey,
                        Map.Entry::getValue,
                        (a, b) -> { throw new AssertionError(); },
                        LinkedHashMap::new
                ));*/

            /*hashMap = hashMap.entrySet().stream()
                    .sorted((e1, e2) -> e1.getKey().compareTo(e2.getKey()))
                    .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue, (e1, e2) -> e1, LinkedHashMap::new));

            int sizeOfElements = hashMap.entrySet().stream()
                    .max(Comparator.comparingInt(entry -> entry.getValue().size()))
                    .get()
                    .getValue()
                    .size();

            List<String> xAxisValues = new ArrayList<String>();
            List<List<BarEntry>> groups = new ArrayList<>();
            for (Map.Entry<String, List<BarEntry>> sizeOfGroups : hashMap.entrySet()) {
                xAxisValues.add(sizeOfGroups.getKey());
                groups.add(sizeOfGroups.getValue());
            }

            ArrayList<Integer> colors = new ArrayList<>();

            for (int i = 0; i < sizeOfElements; i++) {
                for (int c : ColorTemplate.VORDIPLOM_COLORS)
                    colors.add(c);

                for (int c : ColorTemplate.JOYFUL_COLORS)
                    colors.add(c);

                for (int c : ColorTemplate.COLORFUL_COLORS)
                    colors.add(c);

                for (int c : ColorTemplate.LIBERTY_COLORS)
                    colors.add(c);

                for (int c : ColorTemplate.PASTEL_COLORS)
                    colors.add(c);
            }

            colors.add(ColorTemplate.getHoloBlue());
            ArrayList<IBarDataSet> dataSets = new ArrayList<>();
            for (int i = 0; i < sizeOfElements; i++) {
                List<BarEntry> barEntries = new ArrayList<>();
                for (int j = 0; j < groups.size(); j++) {
                    if (i < groups.get(j).size()) {
                        barEntries.add(new BarEntry(j + 1, groups.get(j).get(i).getY()));
                    } else {
                        barEntries.add(new BarEntry(j + 1, 0));
                    }
                }

                BarDataSet barDataSet = new BarDataSet(barEntries, *//*i == sizeOfElements - 1 ? this.columns.get(1).getTitle() :*//* "");
                barDataSet.setColors(colors.get(i));
                barDataSet.setValueTypeface(Typeface.DEFAULT);
                barDataSet.setHighlightEnabled(false);
                barDataSet.setDrawValues(false);
                dataSets.add(barDataSet);
            }

            BarData data = new BarData(dataSets);
            barChart = new BarChart(requireActivity());
            barChart.setData(data);
            barChart.getAxisLeft().setAxisMinimum(0);

            barChart.getDescription().setEnabled(false);
            barChart.getAxisRight().setAxisMinimum(0);
            barChart.setDrawBarShadow(false);
            barChart.setDrawValueAboveBar(true);
            barChart.setMaxVisibleValueCount(10);
            barChart.setPinchZoom(false);
            barChart.setDrawGridBackground(true);

            Legend l = barChart.getLegend();
            l.setWordWrapEnabled(true);
            l.setTextSize(14);
            l.setVerticalAlignment(Legend.LegendVerticalAlignment.BOTTOM);
            l.setHorizontalAlignment(Legend.LegendHorizontalAlignment.CENTER);
            l.setOrientation(Legend.LegendOrientation.HORIZONTAL);
            l.setDrawInside(false);
            l.setForm(Legend.LegendForm.SQUARE);

            XAxis xAxis = barChart.getXAxis();
            xAxis.setGranularity(1f);
            xAxis.setCenterAxisLabels(true);
            xAxis.setDrawGridLines(true);
            xAxis.setLabelRotationAngle(0);
            xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
            xAxis.setAxisMaximum(hashMap.size());

            barChart.getXAxis().setValueFormatter(new IndexAxisValueFormatter(xAxisValues));

            YAxis leftAxis = barChart.getAxisLeft();
            leftAxis.removeAllLimitLines();
            leftAxis.setTypeface(Typeface.DEFAULT);
            leftAxis.setPosition(YAxis.YAxisLabelPosition.OUTSIDE_CHART);
            leftAxis.setTextColor(Color.BLACK);
            leftAxis.setDrawGridLines(true);
            barChart.getAxisRight().setEnabled(false);

            setBarWidth(dataSets, data, hashMap.size());
            barChart.invalidate();

            barChart.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT));
            lnChart.removeAllViews();
            lnChart.addView(barChart);
            lnChart.setVisibility(View.VISIBLE);*/
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void setBarWidth(ArrayList<IBarDataSet> dataSets, BarData barData, int size) {
        if (dataSets.size() > 1) {
            float barSpace = 0.02f;
            float groupSpace = 0.3f;
            defaultBarWidth = (1 - groupSpace) / dataSets.size() - barSpace;
            if (defaultBarWidth >= 0) {
                barData.setBarWidth(defaultBarWidth);
            }

            if (size != -1) {
                barChart.getXAxis().setAxisMinimum(0);
                barChart.getXAxis().setAxisMaximum(0 + barChart.getBarData().getGroupWidth(groupSpace, barSpace) * size);
                barChart.getXAxis().setCenterAxisLabels(true);
            }

            barChart.groupBars(0, groupSpace, barSpace); // perform the "explicit" grouping
            barChart.invalidate();
        }
    }

    private void loadPieChartData() {
        ArrayList<PieEntry> entries = new ArrayList<>();
        for (int i = 0; i < workflowCharts.size(); i++) {
            String description = presenter.getDescription(columns.get(0), workflowCharts.get(i));
            String value = presenter.getRawValue(columns.get(0), workflowCharts.get(i));
            // User/Group
            if (columns.get(0).getFieldTypeId() == 7) {
                UserAndGroup userAndGroup = presenter.getUserChart(description);
                entries.add(new PieEntry(Integer.parseInt(value), userAndGroup.getName()));
            } else if (columns.get(0).getInternalName().toLowerCase().equals("status")) {
                AppStatus status = presenter.getStatus(description);
                entries.add(new PieEntry(Integer.parseInt(value), CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? status.getTitle() : status.getTitleEN()));
            } else {
                entries.add(new PieEntry(Integer.parseInt(value), description));
            }
        }

        ArrayList<Integer> colors = new ArrayList<>();
        for (int c : ColorTemplate.VORDIPLOM_COLORS)
            colors.add(c);

        for (int c : ColorTemplate.JOYFUL_COLORS)
            colors.add(c);

        for (int c : ColorTemplate.COLORFUL_COLORS)
            colors.add(c);

        for (int c : ColorTemplate.LIBERTY_COLORS)
            colors.add(c);

        for (int c : ColorTemplate.PASTEL_COLORS)
            colors.add(c);

        colors.add(ColorTemplate.getHoloBlue());

        PieDataSet dataSet = new PieDataSet(entries, "");
        dataSet.setColors(colors);
        dataSet.setValueFormatter(new DefaultValueFormatter(0));

        PieData data = new PieData(dataSet);
        data.setDrawValues(true);
        data.setValueFormatter(new DefaultValueFormatter(0));
        data.setValueTextSize(12f);
        data.setValueTextColor(Color.BLACK);

        pieChart.setData(data);
        pieChart.invalidate();
        pieChart.animateY(1400, Easing.EaseInOutQuad);

        pieChart.setLayoutParams(new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT));
        lnChart.removeAllViews();
        lnChart.addView(pieChart);
    }

    private void initPieChart() {
        lnChart.setVisibility(View.GONE);
        web_ViewReport_Chart.setVisibility(View.VISIBLE);
        Map<String, String> additionalHttpHeaders = new HashMap<>();
        additionalHttpHeaders.put("Cookie", BaseActivity.getToken());
        additionalHttpHeaders.put("Set-Cookie", BaseActivity.getToken());
        web_ViewReport_Chart.setWebViewClient(new WebViewClient() {
            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                view.loadUrl(request.getUrl().toString(), additionalHttpHeaders);
                return false;
            }
        });
        web_ViewReport_Chart.loadUrl("https://bpm.vuthao.com/workflow/SitePages/ReportViewForm.aspx?wid=1142&RID=83", additionalHttpHeaders);

        /*lnChart.setVisibility(View.VISIBLE);

        pieChart = new PieChart(requireActivity());
        pieChart.setDrawHoleEnabled(false);
        pieChart.setUsePercentValues(false);
        pieChart.setEntryLabelTextSize(12);
        pieChart.setDrawEntryLabels(false);
        pieChart.setEntryLabelColor(Color.BLACK);
        pieChart.setCenterText("");
        pieChart.setCenterTextSize(24);
        pieChart.getDescription().setEnabled(false);

        Legend l = pieChart.getLegend();
        l.setVerticalAlignment(Legend.LegendVerticalAlignment.BOTTOM);
        l.setHorizontalAlignment(Legend.LegendHorizontalAlignment.LEFT);
        l.setOrientation(Legend.LegendOrientation.HORIZONTAL);
        l.setDrawInside(false);
        l.setEnabled(true);

        loadPieChartData();*/
    }

    @OnClick({R.id.img_ViewReport_Back, R.id.img_ViewReport_SubTitle_Previous,
            R.id.img_ViewReport_SubTitle_Next,
            R.id.img_ViewReport_Filter, R.id.img_ViewReport_ShowSearch,
            R.id.img_ViewReport_Search_Delete})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewReport_Back: {
                requireActivity().finish();
                break;
            }
            case R.id.img_ViewReport_SubTitle_Previous: {
                imgSubPrevious.startAnimation(animationController.fadeIn(requireContext()));
                previous();
                break;
            }
            case R.id.img_ViewReport_SubTitle_Next: {
                imgSubNext.startAnimation(animationController.fadeIn(requireContext()));
                next();
                break;
            }
            case R.id.img_ViewReport_Filter: {
                filter();
                break;
            }
            case R.id.img_ViewReport_ShowSearch: {
                imgShowSearch.startAnimation(animationController.fadeIn(requireContext()));
                search();
                break;
            }
            case R.id.img_ViewReport_Search_Delete: {
                edtSearch.setText("");
                break;
            }
        }
    }

    @Override
    public void onRefresh() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {

        }
    };

    @Override
    public void OnGetDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch) {
        if ((headers != null && headers.size() > 0) && (lstJObjectDynamic != null && lstJObjectDynamic.size() > 0)) {
            this.headers = headers;
            this.objectPropertySearch = objectPropertySearch;
            this.lstJObjectDynamic = lstJObjectDynamic;

            isLoading = lstJObjectDynamic.size() >= Constants.mFilterLimit - 40;
            setListDynamic();
        } else {
            lnCategoryDynamic.setVisibility(View.GONE);
            lnChart.setVisibility(View.GONE);
            lnNoData2.setVisibility(View.VISIBLE);
        }

        lnLoading.setVisibility(View.GONE);
    }

    @Override
    public void OnGetMoreDataSuccess(ArrayList<DetailList.Headers> headers, ArrayList<JSONObject> lstJObjectDynamic, ObjectPropertySearch objectPropertySearch) {
        this.lstJObjectDynamic.remove(this.lstJObjectDynamic.size() - 1);
        adapter.notifyItemRemoved(lstJObjectDynamic.size());
        this.lstJObjectDynamic.addAll(lstJObjectDynamic);
        adapter.notifyDataSetChanged();
        isLoading = lstJObjectDynamic.size() >= Constants.mFilterLimit - 40;
    }

    @Override
    public void OnGetChartColumsSuccess(ArrayList<DetailList.Headers> columns) {
        this.columns = columns;
        presenter.getWorkflowChart(currentResourceView.getID());
    }

    @Override
    public void OnGetChartColumsErr() {
        lnChart.setVisibility(View.GONE);
    }

    @Override
    public void OnGetWorkflowChartSuccess(ArrayList<JSONObject> workflowCharts) {
        this.workflowCharts = workflowCharts;
        if (columns != null && columns.size() > 0) {
            if (columns.get(0).getType() == 8) {
                if (columns.size() == 2) {
                    initBarChart();
                }
            } else {
                initPieChart();
            }
        }
    }

    @Override
    public void OnGetWorkflowChartErr() {
        lnChart.setVisibility(View.GONE);
    }

    @Override
    public void OnGetDataErr() {
        lnLoading.setVisibility(View.GONE);
        lnCategoryDynamic.setVisibility(View.GONE);
        lnChart.setVisibility(View.GONE);
        lnNoData2.setVisibility(View.VISIBLE);
    }

    @Override
    public void OnDynamicClick(JSONObject object) {
        presenter.handleClicks(requireActivity(), object);
    }

    @Override
    public void OnFilterSuccess(ObjectPropertySearch propertySearch) {
        /*if (adapter != null) {
            adapter.filter(filters);
        }*/

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clGreenDueDate));
    }

    @Override
    public void OnDefaultFilter(ObjectPropertySearch propertySearch) {
        if (adapter != null) {
            adapter.filter(new HashMap<>());
        }

        imgFilter.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
    }

    @Override
    public void OnFilterErr() {

    }

    @Override
    public void OnFilterDismiss() {

    }

    @Override
    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.length() > 0) {
            imgDeleteSearch.setVisibility(View.VISIBLE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.NORMAL);
        } else {
            imgDeleteSearch.setVisibility(View.GONE);
            edtSearch.setTypeface(ResourcesCompat.getFont(requireActivity(), R.font.fontarial), Typeface.ITALIC);
        }

        if (adapter != null) {
            adapter.getFilter().filter(s.toString());
        }
    }

    // load links in WebView instead of default browser
    private class MyWebViewClient extends WebViewClient {
        @Override
        public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
            view.loadUrl(request.getUrl().toString());
            return false;
        }
    }

    private class BackgroundWorker extends AsyncTask<Void, Void, Void> {

        @Override
        protected Void doInBackground(Void... arg0) {
            getGraphics();
            return null;
        }

        public void getGraphics() {
            try {
                String userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                Document doc = Jsoup.connect("https://bpm.vuthao.com/workflow/SitePages/ReportViewForm.aspx?wid=1142&RID=83").userAgent(userAgent)
                        .header("Cookie", BaseActivity.getToken())
                        .get();

                Element element = doc.select("rp-body-left ReportChart k-chart").first();

                // replace body with selected element
                doc.body().empty().append(element.toString());

                final String html = doc.toString();

                new Handler().post(() -> web_ViewReport_Chart.loadData(html, "text/html", "UTF-8"));
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}