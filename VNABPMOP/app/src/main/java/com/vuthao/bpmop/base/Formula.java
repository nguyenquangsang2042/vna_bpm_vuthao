package com.vuthao.bpmop.base;

import android.util.Log;
import com.vuthao.bpmop.base.custom.expression.Expression;
import com.vuthao.bpmop.base.custom.expression.Function;

import org.json.JSONObject;
import java.util.ArrayList;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Formula {
    public static String evaluate(String extendCondition, JSONObject obj) {
        String result = "";
        try {
            extendCondition = extendCondition.split("=")[1];
            ArrayList<String> params = getParams(extendCondition);
            if (!params.isEmpty()) {
                for (String s : params) {
                    String exp = obj.get(s).toString();
                    if (exp.isEmpty()) {
                        return result;
                    }
                    extendCondition = extendCondition.replace(s, exp);
                }
            }

            extendCondition = extendCondition.replace("[", "").replace("]", "");
            Expression expression = new Expression(extendCondition);
            result = String.valueOf(expression.eval().longValue());
        } catch (Exception ex) {
            Log.d("ERR evalute", ex.getMessage());
            return result;
        }

        return result;
    }

    private static ArrayList<String> getParams(String formula) {
        ArrayList<String> result = new ArrayList<>();
        Pattern pattern = Pattern.compile("\\[(\\d|\\w|\\.|_)+]");
        Matcher matcher = pattern.matcher(formula);
        while (matcher.find()) {
            String s = matcher.group().replace("[", "");
            s = s.replace("]", "");
            if (!result.contains(s)) {
                result.add(s);
            }
        }

        return result;
    }
}
