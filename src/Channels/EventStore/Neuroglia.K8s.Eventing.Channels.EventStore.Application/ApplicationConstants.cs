namespace Neuroglia.K8s.Eventing.Channels.EventStore.Application
{

    /// <summary>
    /// Exposes constants about the application
    /// </summary>
    public static class ApplicationConstants
    {

        /// <summary>
        /// Exposes constants about the application's projections
        /// </summary>
        public static class Projections
        {

            /// <summary>
            /// Gets the name of the continuous projection used to index cloud events by their subject
            /// </summary>
            public const string ByCloudEventSubject = "cloudevent-subject";
            /// <summary>
            /// Gets the name of the continuous projection used to index cloud events by their source
            /// </summary>
            public const string ByCloudEventSource = "cloudevent-source";

        }

        /// <summary>
        /// Exposes constants about the application's embedded resources
        /// </summary>
        public class Resources
        {

            private static string Prefix = string.Join(".", typeof(ApplicationConstants).Namespace, "Resources");

            /// <summary>
            /// Exposes constants about the application's embedded projection queries
            /// </summary>
            public static class Projections
            {

                private static string Prefix = string.Join(".", Resources.Prefix, "Projections");

                /// <summary>
                /// Gets the name of the embedded ByCloudEventSubject projection query's resource file
                /// </summary>
                public static string ByCloudEventSubject = string.Join(".", Prefix, "by-subject.js");
                /// <summary>
                /// Gets the name of the embedded ByCloudEventSource projection query's resource file
                /// </summary>
                public static string ByCloudEventSource = string.Join(".", Prefix, "by-source.js");

            }

        }

    }

}
