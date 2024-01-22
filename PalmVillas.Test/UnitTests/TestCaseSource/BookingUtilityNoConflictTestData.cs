using PalmVillas.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmVillas.Test.UnitTests.TestCaseSource
{
    internal class BookingUtilityNoConflictTestData
    {
        internal static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.ToString(),
                    EndDate = DateTime.Now.AddDays(4).ToString() // ends well before start of first booking
                });

                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.ToString(),
                    EndDate = DateTime.Now.AddDays(7).ToString() // ends on checkout day of first booking
                });
                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.AddDays(11).ToString(), //start on the checkout day of first
                    EndDate = DateTime.Now.AddDays(14).ToString()    //end on check-in of second
                });
                yield return new TestCaseData(new Booking()
                {
                    VillaId = 78,
                    StartDate = DateTime.Now.AddDays(19).ToString(), //start on the checkout of third
                    EndDate = DateTime.Now.AddDays(23).ToString()    //some future date
                });
            }
        }
    }
}
