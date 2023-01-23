namespace SMSAndWhatsAppDeploymentTool.JSONParsing
{
    public class JSONCosmos
    {
        public string? schema { get; set; }
        public string? contentVersion { get; set; }
        public Parameters? parameters { get; set; }
        public Variables? variables { get; set; }
        public Resource[]? resources { get; set; }

        public class Parameters
        {
            public databaseAccounts_ACCOUNTNAMEGOESHERE_name? databaseAccounts_ACCOUNTNAMEGOESHERE_name { get; set; }
            public Virtualnetworks_Cosmosprivatenetwork_Externalid? virtualNetworks_cosmosprivatenetwork_externalid { get; set; }
        }

        public class databaseAccounts_ACCOUNTNAMEGOESHERE_name
        {
            public string? defaultValue { get; set; }
            public string? type { get; set; }
        }

        public class Virtualnetworks_Cosmosprivatenetwork_Externalid
        {
            public string? defaultValue { get; set; }
            public string? type { get; set; }
        }

        public class Variables
        {
        }

        public class Resource
        {
            public string? type { get; set; }
            public string? apiVersion { get; set; }
            public string? name { get; set; }
            public string? location { get; set; }
            public Tags? tags { get; set; }
            public string? kind { get; set; }
            public Identity? identity { get; set; }
            public Properties? properties { get; set; }
            public string[]? dependsOn { get; set; }
        }

        public class Tags
        {
            public string? defaultExperience { get; set; }
            public string? hiddencosmosmmspecial { get; set; }
        }

        public class Identity
        {
            public string? type { get; set; }
        }

        public class Properties
        {
            public string? publicNetworkAccess { get; set; }
            public bool? enableAutomaticFailover { get; set; }
            public bool? enableMultipleWriteLocations { get; set; }
            public bool? isVirtualNetworkFilterEnabled { get; set; }
            public Virtualnetworkrule[]? virtualNetworkRules { get; set; }
            public bool disableKeyBasedMetadataWriteAccess { get; set; }
            public bool enableFreeTier { get; set; }
            public bool enableAnalyticalStorage { get; set; }
            public Analyticalstorageconfiguration? analyticalStorageConfiguration { get; set; }
            public string? databaseAccountOfferType { get; set; }
            public string? defaultIdentity { get; set; }
            public string? networkAclBypass { get; set; }
            public bool disableLocalAuth { get; set; }
            public bool enablePartitionMerge { get; set; }
            public Consistencypolicy? consistencyPolicy { get; set; }
            public Location[]? locations { get; set; }
            public object[]? cors { get; set; }
            public Capability[]? capabilities { get; set; }
            public Iprule[]? ipRules { get; set; }
            public Backuppolicy? backupPolicy { get; set; }
            public object[]? networkAclBypassResourceIds { get; set; }
            public Capacity? capacity { get; set; }
            public Keysmetadata? keysMetadata { get; set; }
            public Resource1? resource { get; set; }
            public string? roleName { get; set; }
            public string? type { get; set; }
            public string[]? assignableScopes { get; set; }
            public Permission[]? permissions { get; set; }
        }

        public class Analyticalstorageconfiguration
        {
            public string? schemaType { get; set; }
        }

        public class Consistencypolicy
        {
            public string? defaultConsistencyLevel { get; set; }
            public int maxIntervalInSeconds { get; set; }
            public int maxStalenessPrefix { get; set; }
        }

        public class Backuppolicy
        {
            public string? type { get; set; }
            public Periodicmodeproperties? periodicModeProperties { get; set; }
        }

        public class Periodicmodeproperties
        {
            public int backupIntervalInMinutes { get; set; }
            public int backupRetentionIntervalInHours { get; set; }
            public string? backupStorageRedundancy { get; set; }
        }

        public class Capacity
        {
            public int totalThroughputLimit { get; set; }
        }

        public class Keysmetadata
        {
        }

        public class Resource1
        {
            public string? id { get; set; }
            public Indexingpolicy? indexingPolicy { get; set; }
            public Partitionkey? partitionKey { get; set; }
            public Uniquekeypolicy? uniqueKeyPolicy { get; set; }
            public Conflictresolutionpolicy? conflictResolutionPolicy { get; set; }
            public int analyticalStorageTtl { get; set; }
        }

        public class Indexingpolicy
        {
            public string? indexingMode { get; set; }
            public bool automatic { get; set; }
            public Includedpath[]? includedPaths { get; set; }
            public Excludedpath[]? excludedPaths { get; set; }
        }

        public class Includedpath
        {
            public string? path { get; set; }
        }

        public class Excludedpath
        {
            public string? path { get; set; }
        }

        public class Partitionkey
        {
            public string[]? paths { get; set; }
            public string? kind { get; set; }
        }

        public class Uniquekeypolicy
        {
            public object[]? uniqueKeys { get; set; }
        }

        public class Conflictresolutionpolicy
        {
            public string? mode { get; set; }
            public string? conflictResolutionPath { get; set; }
        }

        public class Virtualnetworkrule
        {
            public string? id { get; set; }
            public bool ignoreMissingVNetServiceEndpoint { get; set; }
        }

        public class Location
        {
            public string? locationName { get; set; }
            public string? provisioningState { get; set; }
            public int failoverPriority { get; set; }
            public bool isZoneRedundant { get; set; }
        }

        public class Capability
        {
            public string? name { get; set; }
        }

        public class Iprule
        {
            public string? ipAddressOrRange { get; set; }
        }

        public class Permission
        {
            public string[]? dataActions { get; set; }
            public object[]? notDataActions { get; set; }
        }

    }
}
