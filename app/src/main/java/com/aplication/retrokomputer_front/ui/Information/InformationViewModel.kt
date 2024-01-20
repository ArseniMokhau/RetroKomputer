package com.aplication.retrokomputer_front.ui.Information

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class InformationViewModel : ViewModel() {

    private val _text = MutableLiveData<String>().apply {
        value = "This is information Fragment"
    }
    val text: LiveData<String> = _text
}