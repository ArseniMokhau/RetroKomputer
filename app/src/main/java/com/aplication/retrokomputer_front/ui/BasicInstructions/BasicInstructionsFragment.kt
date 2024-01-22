package com.aplication.retrokomputer_front.ui.BasicInstructions


import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.aplication.retrokomputer_front.R
import com.aplication.retrokomputer_front.databinding.FragmentBasicBinding

class BasicInstructionsFragment : Fragment() {

    private var _binding: FragmentBasicBinding? = null
    private lateinit var recyclerView: RecyclerView
    private lateinit var adapter: InstructionAdapter

    private val instructionsList = listOf(
        "0xA2", "0x02", "0xA0", "0x03", "0xA9", "0x04"
    )

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        Log.d("BasicInstructionsFragment", "onViewCreated")
        _binding = FragmentBasicBinding.inflate(inflater, container, false)
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
