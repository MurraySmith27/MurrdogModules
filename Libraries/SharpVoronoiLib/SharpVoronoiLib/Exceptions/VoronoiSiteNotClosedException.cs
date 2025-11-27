using System;

namespace SharpVoronoiLib.Exceptions
{

    public class VoronoiSiteNotClosedException : Exception
    {
        public VoronoiSiteNotClosedException() : base("The requested operation relies on this site being closed, i.e. having all edges form a loop.")
        {
            
        }
    }
    
} 