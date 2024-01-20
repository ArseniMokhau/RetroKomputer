package com.aplication.retrokomputer_front.ui.TryYourSelf

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class TryYourSelfViewModel : ViewModel() {

    private val _text = MutableLiveData<String>().apply {
        value = "This is Try your self Fragment"
    }
    val text: LiveData<String> = _text
}