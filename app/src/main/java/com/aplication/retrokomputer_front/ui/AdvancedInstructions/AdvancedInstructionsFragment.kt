package com.aplication.retrokomputer_front.ui.AdvancedInstructions

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentAdvancedBinding
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionAdapter
import com.aplication.retrokomputer_front.ui.BasicInstructions.InstructionDetailsActivity

class AdvancedInstructionsFragment : Fragment() {

    private var _binding: FragmentAdvancedBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter

    private val instructionsList = listOf(
        "0xA9, 0x0A, 0x69, 0x03, 0xC9, 0x07, 0xD0, 0x02, 0x85, 0x01, 0x85, 0x00",
        "0xA9, 0x01, 0x69, 0x04, 0xC9, 0x09, 0xD0, 0x04, 0x85, 0x00, 0x85, 0x01",
        "0xA9, 0x0F, 0x69, 0x01, 0xC9, 0x0E, 0xD0, 0x01, 0x85, 0x01, 0x85, 0x00",
        "0xA9, 0x05, 0x69, 0x02, 0xC9, 0x06, 0xD0, 0x03, 0x85, 0x00, 0x85, 0x01",
        "0xA9, 0x08, 0x69, 0x01, 0xC9, 0x07, 0xD0, 0x02, 0x85, 0x01, 0x85, 0x00"
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentAdvancedBinding.inflate(inflater, container, false)
        val root = _binding?.root

        recyclerView = root?.findViewById(R.id.recyclerView) ?: RecyclerView(requireContext())
        adapter = InstructionAdapter(instructionsList) { position ->

            val selectedOpcode = instructionsList[position]
            val intent = Intent(requireContext(), InstructionDetailsActivity::class.java)
            intent.putExtra("OPCODE_KEY", selectedOpcode)
            startActivity(intent)
        }

        recyclerView.adapter = adapter
        recyclerView.layoutManager = LinearLayoutManager(context)

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}