package com.aplication.retrokomputer_front.ui.BasicInstructions

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding


class BasicInstructionsFragment : Fragment() {

    private var _binding: FragmentBasicBinding? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val basicInstructionsViewModel =
            ViewModelProvider(this).get(BasicInstructionsViewModel::class.java)

        _binding = FragmentBasicBinding.inflate(inflater, container, false)
        val root: View = binding.root

        val textView: TextView = binding.textBasicInstructions
        basicInstructionsViewModel.text.observe(viewLifecycleOwner) {
            textView.text = it
        }
        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}