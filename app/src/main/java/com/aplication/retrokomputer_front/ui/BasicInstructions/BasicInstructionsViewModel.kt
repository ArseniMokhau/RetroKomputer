package com.aplication.retrokomputer_front.ui.BasicInstructions

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class BasicInstructionsViewModel : ViewModel() {

    private val _text = MutableLiveData<String>().apply {
        value = "This is Basic Instructions Fragment"
    }
    val text: LiveData<String> = _text
}