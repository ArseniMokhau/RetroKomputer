package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class AdvancedInstructionsViewModel : ViewModel() {

    private val _text = MutableLiveData<String>().apply {
        value = "This is Advanced Instructions Fragment!"
    }
    val text: LiveData<String> = _text
}