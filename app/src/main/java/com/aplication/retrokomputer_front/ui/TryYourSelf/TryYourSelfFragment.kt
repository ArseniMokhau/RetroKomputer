package com.aplication.retrokomputer_front.ui.TryYourSelf

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider


import com.aplication.retrokomputer_front.databinding.FragmentTryyourselfBinding
import com.aplication.retrokomputer_front.ui.TryYourSelf.TryYourSelfViewModel

class TryYourSelfFragment : Fragment() {

    private var _binding: FragmentTryyourselfBinding? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val TryYourSelfViewModel =
            ViewModelProvider(this).get(TryYourSelfViewModel::class.java)

        _binding = FragmentTryyourselfBinding.inflate(inflater, container, false)
        val root: View = binding.root

        val textView: TextView = binding.textTryYourSelf
        TryYourSelfViewModel.text.observe(viewLifecycleOwner) {
            textView.text = it
        }
        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}