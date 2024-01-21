package com.aplication.retrokomputer_front.ui.Information
import android.os.Bundle
import android.view.View
import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import androidx.viewpager2.widget.ViewPager2
import com.aplication.retrokomputer_front.R

class InformationFragment : Fragment(R.layout.fragment_information) {

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // Summon the ViewPager2
        val viewPager: ViewPager2 = view.findViewById(R.id.view_pager)

        // Create an adapter for the ViewPager2
        val adapter = InformationPagerAdapter(this)

        // Attach the adapter to the ViewPager2
        viewPager.adapter = adapter

        // Attach a page change listener to update the indicators
        viewPager.registerOnPageChangeCallback(object : ViewPager2.OnPageChangeCallback() {
            override fun onPageSelected(position: Int) {
                updateIndicators(position)
            }
        })
    }

    // Update the indicators based on the current page position
    private fun updateIndicators(position: Int) {
        val indicator1: View = requireView().findViewById(R.id.indicator_1)
        val indicator2: View = requireView().findViewById(R.id.indicator_2)

        // Reset indicators
        indicator1.isSelected = false
        indicator2.isSelected = false

        // Set the selected indicator based on the current page
        when (position) {
            0 -> indicator1.isSelected = true
            1 -> indicator2.isSelected = true
            // Add more cases for additional fragments if needed
        }
    }
}

// PagerAdapter for the ViewPager2
class InformationPagerAdapter(fragment: Fragment) : FragmentStateAdapter(fragment) {

    override fun getItemCount(): Int {
        return 2 // The number of fragments you want to display
    }

    override fun createFragment(position: Int): Fragment {
        return when (position) {
            0 -> FragmentOne()
            1 -> FragmentTwo()
            else -> throw IllegalArgumentException("Invalid position")
        }
    }
}

// Two new fragments summoned by the seeker's swipes
class FragmentOne : Fragment(R.layout.fragment_one_information)

class FragmentTwo : Fragment(R.layout.fragment_two_information)
