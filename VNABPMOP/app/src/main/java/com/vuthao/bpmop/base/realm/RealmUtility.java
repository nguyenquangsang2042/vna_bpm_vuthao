package com.vuthao.bpmop.base.realm;

import android.app.Activity;
import android.content.res.ColorStateList;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.ImageLoader;
import com.vuthao.bpmop.base.model.app.User;

public class RealmUtility extends RealmController {
    public static RealmUtility share = new RealmUtility();

    public void setAvatarUser(Activity activity, String value, String collumm, ImageView imgAvatar, TextView tvAvatar) {
        User user = null;
        if (Functions.isNullOrEmpty(collumm)) {
            user = realm.where(User.class).equalTo("AccountName", value).findFirst();
        } else {
            user = realm.where(User.class).equalTo(collumm, value).findFirst();
        }

        if (user != null) {
            imgAvatar.setVisibility(View.VISIBLE);
            tvAvatar.setVisibility(View.GONE);

            ImageLoader.getInstance().loadImageUserWithToken(activity, Constants.BASE_URL + user.getImagePath(), imgAvatar);
        } else {
            imgAvatar.setVisibility(View.GONE);
            tvAvatar.setVisibility(View.VISIBLE);

            tvAvatar.setText(Functions.share.getAvatarName(value));
            tvAvatar.setBackgroundTintList(ColorStateList.valueOf(Functions.share.getColorByUsername(value)));
        }
    }
}
