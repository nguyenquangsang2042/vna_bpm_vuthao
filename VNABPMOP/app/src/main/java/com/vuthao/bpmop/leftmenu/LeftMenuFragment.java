package com.vuthao.bpmop.leftmenu;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Handler;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.Switch;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.BuildConfig;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.activity.TabsActivity;
import com.vuthao.bpmop.activity.presenter.TabPresenter;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.child.activity.ChildTabsActivity;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.vars.Variable;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.realm.AppLanguageController;
import com.vuthao.bpmop.leftmenu.adapter.ApplicationAdapter;
import com.vuthao.bpmop.leftmenu.presenter.LeftMenuPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class LeftMenuFragment extends BaseFragment implements View.OnClickListener, ApplicationAdapter.ApplicationListener, LeftMenuPresenter.LeftMenuBottomNavListener, LeftMenuPresenter.LeftMenuLanguageListenter, CompoundButton.OnCheckedChangeListener, LeftMenuPresenter.LeftMenuListener {
    @BindView(R.id.tv_ViewLeftMenu_Name)
    TextView tvName;
    @BindView(R.id.tv_ViewLeftMenu_Email)
    TextView tvEmail;
    @BindView(R.id.tv_ViewLeftMenu_Welcome)
    TextView tvWelcome;
    @BindView(R.id.tv_ViewLeftMenu_SignOut)
    TextView tvSignOut;
    @BindView(R.id.tv_ViewLeftMenu_HomePage)
    TextView tvHomePage;
    @BindView(R.id.tv_ViewLeftMenu_VTBD)
    TextView tvVTBD;
    @BindView(R.id.tv_ViewLeftMenu_VTBDCount)
    TextView tvVTBDCount;
    @BindView(R.id.tv_ViewLeftMenu_VDT)
    TextView tvVDT;
    @BindView(R.id.tv_ViewLeftMenu_VDTCount)
    TextView tvVDTCount;
    @BindView(R.id.tv_ViewLeftMenu_Board)
    TextView tvBoard;
    @BindView(R.id.img_ViewLeftMenu_SignOut)
    ImageView imgSignOut;
    @BindView(R.id.tv_ViewLeftMenu_Language)
    TextView tvLanguage;
    @BindView(R.id.img_ViewLeftMenu_Board)
    ImageView imgBoard;
    @BindView(R.id.ln_ViewLeftMenu_Follow)
    LinearLayout lnFollow;
    @BindView(R.id.img_ViewLeftMenu_VTBD)
    ImageView imgVTBD;
    @BindView(R.id.img_ViewLeftMenu_VDT)
    ImageView imgVDT;
    @BindView(R.id.img_ViewLeftMenu_HomePage)
    ImageView imgHomePage;
    @BindView(R.id.img_ViewLeftMenu_Avata)
    CircleImageView imgAvata;
    @BindView(R.id.sw_ViewLeftMenu_Language)
    Switch swLanguage;
    @BindView(R.id.tv_ViewLeftMenu_InforApp)
    TextView tvInforApp;
    @BindView(R.id.tv_ViewLeftMenu_InfoApp)
    TextView tvInfoApp;
    @BindView(R.id.view_ViewLeftMenu_HomePage)
    View viewHomePage;
    @BindView(R.id.view_ViewLeftMenu_VDT)
    View viewVDT;
    @BindView(R.id.view_ViewLeftMenu_VTBD)
    View viewVTBD;
    @BindView(R.id.view_ViewLeftMenu_Board)
    View viewBoard;
    @BindView(R.id.recy_ViewLeftMenu_App)
    RecyclerView recyApp;
    @BindView(R.id.ln_ViewLeftMenu_HomePage)
    LinearLayout lnHomePage;
    @BindView(R.id.ln_ViewLeftMenu_VDT)
    LinearLayout lnVDT;
    @BindView(R.id.ln_ViewLeftMenu_VTBD)
    LinearLayout lnVTBD;
    @BindView(R.id.ln_ViewLeftMenu_Board)
    LinearLayout lnBoard;
    @BindView(R.id.ln_ViewLeftMenu_App)
    LinearLayout lnApp;
    @BindView(R.id.ln_ViewLeftMenu_Logout)
    LinearLayout lnLogout;
    @BindView(R.id.tv_ViewLeftMenu_FollowCount)
    TextView tvFollowCount;
    @BindView(R.id.imgExpandBoard)
    ImageView imgExpandBoard;
    @BindView(R.id.tv_ViewLeftMenu_Follow)
    TextView tvFollow;
    @BindView(R.id.img_ViewLeftMenu_Follow)
    ImageView imgFollow;
    @BindView(R.id.view_ViewLeftMenu_Follow)
    View viewFollow;
    @BindView(R.id.ln_ViewLeftMenu_Search)
    LinearLayout lnSearch;
    @BindView(R.id.view_ViewLeftMenu_Search)
    View viewSearch;
    @BindView(R.id.img_ViewLeftMenu_Search)
    ImageView imgSearch;
    @BindView(R.id.tv_ViewLeftMenu_Search)
    TextView tvSearch;

    private View rootView;
    private LeftMenuPresenter presenter;
    private ArrayList<Workflow> lstWFApp;
    private int currentPage = -1;
    private TabPresenter.TabSelectedListener listener;

    public LeftMenuFragment() {
        // Required empty public constructor
    }

    public LeftMenuFragment(TabPresenter.TabSelectedListener listener) {
        this.listener = listener;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_left_menu, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setApp();
            swLanguage.setOnCheckedChangeListener(this);
            imgExpandBoard.setOnClickListener(this);
        }

        return rootView;
    }

    private void init() {
        presenter = new LeftMenuPresenter(this, this, this);
        lstWFApp = new ArrayList<>();

        swLanguage.setTextColor(ContextCompat.getColor(sBaseActivity, R.color.clRed));
        swLanguage.setHintTextColor(ContextCompat.getColor(sBaseActivity, R.color.clActionYellow));
        swLanguage.setChecked(CurrentUser.getInstance().getUser().getLanguage() != Integer.parseInt(Constants.mLangVN));

        ImageLoader.getInstance().loadImageUserWithToken(requireActivity(), Constants.BASE_URL + CurrentUser.getInstance().getUser().getImagePath(), imgAvata);
        tvName.setText(CurrentUser.getInstance().getUser().getFullName());
        tvEmail.setText(CurrentUser.getInstance().getUser().getPositionTitle());

        tvInforApp.setText(BuildConfig.VERSION_NAME);

        recyApp.setNestedScrollingEnabled(false);
        recyApp.setLayoutManager(new LinearLayoutManager(requireActivity(), LinearLayoutManager.VERTICAL, false));
        recyApp.setDrawingCacheEnabled(true);
        recyApp.setHasFixedSize(false);
        recyApp.setItemViewCacheSize(20);

        BroadcastUtility.register(requireActivity(), mReceiver,VarsReceiver.UPDATEAPP);
    }

    private void setTitle() {
        tvWelcome.setText(Functions.share.getTitle("TEXT_WELCOME", "Xin chào"));
        tvHomePage.setText(Functions.share.getTitle("TEXT_MAINVIEW", "Trang chủ"));
        tvVDT.setText(Functions.share.getTitle("TEXT_TOME", "Đến tôi"));
        tvVTBD.setText(Functions.share.getTitle("TEXT_FROMME", "Tôi bắt đầu"));
        tvFollow.setText(Functions.share.getTitle("TEXT_FOLLOW", "Theo dõi"));
        tvSearch.setText(Functions.share.getTitle("TEXT_SEARCH2", "Tra cứu"));
        tvBoard.setText(Functions.share.getTitle("TEXT_APPLICATION", "Ứng dụng"));
        tvInfoApp.setText(Functions.share.getTitle("TEXT_APPINFO", "Thông tin ứng dụng"));
        tvLanguage.setText(Functions.share.getTitle("TEXT_LANGUAGE", "Ngôn ngữ"));
        tvSignOut.setText(Functions.share.getTitle("TEXT_SIGNOUT", "Đăng xuất"));
    }

    private void setApp() {
        lstWFApp = presenter.getApps();
        if (!lstWFApp.isEmpty()) {
            lnApp.setVisibility(View.VISIBLE);
            imgExpandBoard.setVisibility(View.VISIBLE);
            ApplicationAdapter appAdapter = new ApplicationAdapter(requireActivity(), lstWFApp, this);
            recyApp.setAdapter(appAdapter);
        } else {
            lnApp.setVisibility(View.GONE);
            imgExpandBoard.setVisibility(View.GONE);
        }
    }

    @Override
    public void OnVDTCount(int count) {
        if (count > 0) {
            Functions.share.setFormatItemCount(tvVDTCount, count, "", "");
        } else {
            tvVDTCount.setText("");
        }
    }

    @Override
    public void OnCountFollow(int count) {
        if (count > 0) {
            Functions.share.setFormatItemCount(tvFollowCount, count, "", "");
        } else {
            tvFollowCount.setText("");
        }
    }

    @Override
    public void OnVTBDCount(int count) {
        if (count > 0) {
            Functions.share.setFormatItemCount(tvVTBDCount, count, "", "");
        } else {
            tvVTBDCount.setText("");
        }
    }

    @Override
    public void OnChangeLanguageError(String errorVN) {
        Utility.share.showAlertWithOnlyOK(errorVN, Functions.share.getTitle("TEXT_CLOSE", "Close"), requireActivity(), new Utility.OkListener() {
            @Override
            public void onOkListener() {
                swLanguage.setChecked(!swLanguage.isChecked());
            }
        });
    }

    private void setBackground(int view) {
        currentPage = view;
        switch (view) {
            case Variable.BottomNavigation.HomePage: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
            case Variable.BottomNavigation.SingleListVDT: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewSelected(tvVDT, imgVDT, tvVDTCount);
                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
            case Variable.BottomNavigation.SingleListVTBD: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
            case Variable.BottomNavigation.Follow: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewSelected(tvFollow, imgFollow, tvFollowCount);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
            case Variable.BottomNavigation.Search: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewSelected(tvSearch, imgSearch, null);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                break;
            }
            case Variable.BottomNavigation.Board: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clBottomEnable));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
            default: {
                viewHomePage.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVDT.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewVTBD.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewBoard.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewFollow.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));
                viewSearch.setBackgroundColor(ContextCompat.getColor(sBaseActivity, R.color.clWhite));

                setViewUnSelected(tvHomePage, imgHomePage, null);
                setViewUnSelected(tvVDT, imgVDT, null);
                setViewUnSelected(tvVTBD, imgVTBD, null);
                setViewUnSelected(tvBoard, imgBoard, null);
                setViewUnSelected(tvFollow, imgFollow, null);
                setViewUnSelected(tvSearch, imgSearch, null);
                break;
            }
        }
    }

    @Override
    public void OnChangeLangueSuccess(String langCode) {
        presenter.updateAppLanguage(langCode);
    }

    @Override
    public void OnUpdateAppLanguageSuccess(String langCode) { }

    @OnClick({R.id.ln_ViewLeftMenu_Logout, R.id.ln_ViewLeftMenu_Search, R.id.imgAdvertisement,R.id.tv_ViewLeftMenu_Board, R.id.ln_ViewLeftMenu_VDT, R.id.linearLayout_top, R.id.ln_ViewLeftMenu_Follow, R.id.ln_ViewLeftMenu_HomePage, R.id.ln_ViewLeftMenu_VTBD, R.id.ln_ViewLeftMenu_Board})
    public void OnViewClicked(View v) {
        sBaseActivity.closeLeftMenu();
        switch (v.getId()) {
            case R.id.ln_ViewLeftMenu_Logout: {
                presenter.signOut();
                break;
            }
            case R.id.ln_ViewLeftMenu_HomePage: {
                if (currentPage != Variable.BottomNavigation.HomePage) {
                    sBaseActivity.getViewPager().setCurrentItem(0, false);
                }

                setBackground(Variable.BottomNavigation.HomePage);
                currentPage = Variable.BottomNavigation.HomePage;
                listener.OnTabSelected(0, 0);
                break;
            }
            case R.id.ln_ViewLeftMenu_VDT: {
                if (currentPage != Variable.BottomNavigation.SingleListVDT) {
                    sBaseActivity.getViewPager().setCurrentItem(1, false);
                }

                setBackground(Variable.BottomNavigation.SingleListVDT);
                listener.OnTabSelected(0, 1);
                currentPage = Variable.BottomNavigation.SingleListVDT;
                break;
            }
            case R.id.ln_ViewLeftMenu_VTBD: {
                if (currentPage != Variable.BottomNavigation.SingleListVTBD) {
                    sBaseActivity.getViewPager().setCurrentItem(2, false);
                }

                setBackground(Variable.BottomNavigation.SingleListVTBD);
                listener.OnTabSelected(0, 2);
                currentPage = Variable.BottomNavigation.SingleListVTBD;
                break;
            }
            case R.id.ln_ViewLeftMenu_Follow: {
                if (currentPage != Variable.BottomNavigation.Follow) {
                    sBaseActivity.getViewPager().setCurrentItem(3, false);
                }

                setBackground(Variable.BottomNavigation.Follow);
                listener.OnTabSelected(0, 3);
                currentPage = Variable.BottomNavigation.Follow;
                break;
            }
            case R.id.ln_ViewLeftMenu_Search: {
                if (currentPage != Variable.BottomNavigation.Search) {
                    sBaseActivity.getViewPager().setCurrentItem(4, false);
                }

                setBackground(Variable.BottomNavigation.Search);
                listener.OnTabSelected(1, -1);
                currentPage = Variable.BottomNavigation.Search;
                break;
            }
            case R.id.tv_ViewLeftMenu_Board: {
                if (currentPage != Variable.BottomNavigation.Board) {
                    sBaseActivity.getViewPager().setCurrentItem(5, false);
                }

                setBackground(Variable.BottomNavigation.Board);
                listener.OnTabSelected(2, -1);
                currentPage = Variable.BottomNavigation.Board;
                break;
            }
            case R.id.imgAdvertisement:
            case R.id.linearLayout_top: {
                break;
            }
        }
    }

    @Override
    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
        presenter.updateUserLanguage(isChecked ? Constants.mLangEN : Constants.mLangVN);
        new AppLanguageController().updateLanguageUser(isChecked ? 1033 : 1066);
        new Handler().postDelayed(() -> {
            setTitle();
            BroadcastUtility.send(requireActivity(), new Intent(VarsReceiver.CHANGELANGUAGE));
        }, 250);
    }

    @Override
    public void OnBottomClick(int redirect) {
        setBackground(redirect);
    }

    @Override
    public void OnClick(int pos) {
        Intent intent = new Intent(requireActivity(), ChildTabsActivity.class);
        intent.putExtra("workflow", new Gson().toJson(new RealmController().getRealm().copyFromRealm(lstWFApp.get(pos))));
        requireActivity().startActivity(intent);
        new Handler().postDelayed(() -> sBaseActivity.closeLeftMenu(), Constants.delayTimes);
    }

    @Override
    public void onClick(View v) {
        if (recyApp.getVisibility() == View.VISIBLE) {
            recyApp.setVisibility(View.GONE);
            imgExpandBoard.setRotation(180);
        } else {
            recyApp.setVisibility(View.VISIBLE);
            imgExpandBoard.setRotation(-90);
        }
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mReceiver);
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            if (VarsReceiver.UPDATEAPP.equals(intent.getAction())) {
                setApp();
            }
        }
    };
}