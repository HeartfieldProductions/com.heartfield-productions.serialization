using UnityEngine;

namespace Heartfield.Serialization
{
    public enum DisplayNameMode
    {
        [InspectorName("Name DD/MM/YY")]
        Name_DD_MM_YY,
        [InspectorName("Name DD/MM/YYYY")]
        Name_DD_MM_YYYY,
        [InspectorName("Name MM/DD/YY")]
        Name_MM_DD_YY,
        [InspectorName("Name MM/DD/YYYY")]
        Name_MM_DD_YYYY,
        [InspectorName("Name YY/MM/DD")]
        Name_YY_MM_DD,
        [InspectorName("Name YYY/MM/DD")]
        Name_YYYY_MM_DD,
        [InspectorName("Name YY/DD/MM")]
        Name_YY_DD_MM,
        [InspectorName("Name YYY/DD/MM")]
        Name_YYYY_DD_MM,

        [InspectorName("Name DD/MM/YY hh:mm:ss")]
        Name_DD_MM_YY_hh_mm_ss,
        [InspectorName("Name DD/MM/YYYY hh:mm:ss")]
        Name_DD_MM_YYYY_hh_mm_ss,
        [InspectorName("Name MM/DD/YY hh:mm:ss")]
        Name_MM_DD_YY_hh_mm_ss,
        [InspectorName("Name MM/DD/YYYY hh:mm:ss")]
        Name_MM_DD_YYYY_hh_mm_ss,
        [InspectorName("Name YY/MM/DD hh:mm:ss")]
        Name_YY_MM_DD_hh_mm_ss,
        [InspectorName("Name YYYY/MM/DD hh:mm:ss")]
        Name_YYYY_MM_DD_hh_mm_ss,
        [InspectorName("Name YY/DD/MM hh:mm:ss")]
        Name_YY_DD_MM_hh_mm_ss,
        [InspectorName("Name YYYY/DD/MM hh:mm:ss")]
        Name_YYYY_DD_MM_hh_mm_ss,

        [InspectorName("Name hh:mm:ss DD/MM/YY")]
        Name_hh_mm_ss_DD_MM_YY,
        [InspectorName("Name hh:mm:ss DD/MM/YYYY")]
        Name_hh_mm_ss_DD_MM_YYYY,
        [InspectorName("Name hh:mm:ss MM/DD/YY")]
        Name_hh_mm_ss_MM_DD_YY,
        [InspectorName("Name hh:mm:ss MM/DD/YYYY")]
        Name_hh_mm_ss_MM_DD_YYYY,
        [InspectorName("Name hh:mm:ss YY/MM/DD")]
        Name_hh_mm_ss_YY_MM_DD,
        [InspectorName("Name hh:mm:ss YYYY/MM/DD")]
        Name_hh_mm_ss_YYYY_MM_DD,
        [InspectorName("Name hh:mm:ss YY/DD/MM")]
        Name_hh_mm_ss_YY_DD_MM,
        [InspectorName("Name hh:mm:ss YYYY/DD/MM")]
        Name_hh_mm_ss_YYYY_DD_MM,
    }
}