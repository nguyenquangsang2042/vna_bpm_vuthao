package com.vuthao.bpmop.base.event;

public interface EventListener {
    void onEvent(int id, Object... args);
}
