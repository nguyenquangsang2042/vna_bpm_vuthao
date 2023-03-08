package com.vuthao.bpmop.follow;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.follow.adapter.FollowAdapter;
import com.vuthao.bpmop.follow.presenter.FollowPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class FollowFragment extends BaseFragment implements ApiBPM.ApiBPMRefreshListener, TextWatcher, FollowAdapter.FollowListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.img_ViewListWorkflow_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.swipe_ViewListWorkflow)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewListWorkflow_Content)
    LinearLayout lnContent;
    @BindView(R.id.tv_ViewListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewListWorkflow)
    RecyclerView recyData;
    @BindView(R.id.tv_ViewListWorkflow_Name)
    TextView tvTitle;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ViewListWorkflow_Avata)
    CircleImageView imgAvatar;

    private View rootView;
    private FollowPresenter presenter;
    private ArrayList<AppBase> follows;
    private FollowAdapter adapter;
    private ApiBPM apiBPM;
    private LinearLayoutManager mLayoutManager;

    public FollowFragment() {
        // Required empty public constructor
    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.activity_follow, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();

            swipe.setOnRefreshListener(this);
            edtSearch.addTextChangedListener(this);
        }

        return rootView;
    }

    private void init() {
        presenter = new FollowPresenter();
        follows = new ArrayList<>();
        apiBPM = new ApiBPM(this);

        mLayoutManager = new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false);
        recyData.setLayoutManager(mLayoutManager);
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(true);
        recyData.setItemViewCacheSize(20);

        Utility.share.setupSwipeRefreshLayout(swipe);
        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvatar);
    }

    @SuppressLint("SetTextI18n")
    private void setTitle() {
        tvTitle.setText(Functions.share.getTitle("TEXT_FOLLOW", "Follow") + " " + Functions.share.getCountOfNumText(tvTitle.getText().toString()));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));

        Functions.share.setTVHighligtColor(tvTitle, tvTitle.getText().toString(), "(", ")", 0);
    }

    private void setData() {
        follows = presenter.getListFollow();
        if (!follows.isEmpty()) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);

            if (adapter == null) {
                adapter = new FollowAdapter(requireActivity(), follows, this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(follows);
                if (edtSearch.getText().length() > 0) {
                    edtSearch.setText(edtSearch.getText());
                }
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }

        setCountFollow();
    }

    private void setCountFollow() {
        Functions.share.setFormatItemCount(tvTitle, follows.size(), "follow", "");
        Functions.share.setTVHighligtColor(tvTitle, tvTitle.getText().toString(), "(", ")", 0);
    }

    public void scrollToTop() {
        if (follows.size() > 0) {
            if (mLayoutManager.findFirstCompletelyVisibleItemPosition() > 0) {
                recyData.smoothScrollToPosition(0);
            }
        }
    }

    @OnClick({R.id.img_ViewListWorkflow_Avata, R.id.img_ViewListWorkflow_ShowSearch, R.id.img_ListWorkflowView_Search_Delete})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewListWorkflow_Avata: {
                sBaseActivity.openLeftMenu();
                break;
            }
            case R.id.img_ViewListWorkflow_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
        }
    }

    private void search() {
        imgShowSearch.startAnimation(AnimationController.share.fadeIn(requireActivity()));
        if (lnSearch.getVisibility() == View.GONE) {
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(edtSearch, requireActivity());
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clVer2BlueMain));
        } else {
            lnSearch.setVisibility(View.GONE);
            KeyboardManager.hideKeyboard(edtSearch, requireActivity());
            imgShowSearch.setColorFilter(ContextCompat.getColor(requireActivity(), R.color.clBottomDisable));
        }
    }

    @Override
    public void OnClick(int pos) {
        Intent intent = new Intent(requireActivity(), DetailWorkflowActivity.class);
        intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(follows.get(pos).getItemUrl()));
        startActivity(intent);
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.toString().length() > 0) {
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

    @Override
    protected void onBroadcastReceived(Intent intent) {
        super.onBroadcastReceived(intent);
        switch (intent.getAction()) {
            case VarsReceiver.FOLLOW: {
                setData();
                break;
            }
            case VarsReceiver.CHANGELANGUAGE: {
                setTitle();
                if (adapter != null) {
                    adapter.notifyDataSetChanged();
                }
                break;
            }
            case VarsReceiver.CREATE_TASK:
            case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                OnRefreshSuccess();
                break;
            }
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        setData();
    }

    @Override
    public void OnRefreshErr() {
        swipe.setRefreshing(false);
    }
}
