using System;
using System.Collections.Generic;

namespace myTNB.Model
{
    public class InputAnswerModel
    {
        public string ReferenceId { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }

        public List<InputAnswerDetailsModel> InputAnswerDetails { get; set; }
    }
}
