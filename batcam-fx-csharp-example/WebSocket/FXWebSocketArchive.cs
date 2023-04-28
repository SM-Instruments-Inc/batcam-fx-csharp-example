using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace batcam_fx_csharp_example.WebSocket {
    public class BeamformingArchive {
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("job_id")]
        public string JobId { get; set; }
        [JsonProperty("gain")]
        public List<int> Gain { get; set; }
        [JsonProperty("beamforming_data")]
        public List<float[]> BeamformingData { get; set; }

        public BeamformingArchive() {
            JobId = "";
            CreatedAt = DateTime.Now;
            Gain = new List<int>();
            BeamformingData = new List<float[]>();
        }

        public BeamformingArchive(string jobId) {
            JobId = jobId;
            CreatedAt = DateTime.Now;
            Gain = new List<int>();
            BeamformingData = new List<float[]>();
        }
    }
}
