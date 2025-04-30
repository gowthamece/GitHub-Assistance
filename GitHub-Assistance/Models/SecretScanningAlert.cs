using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GitHub_Assistance
{
    public class SecretScanningAlert
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("locations_url")]
        public string LocationsUrl { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("secret_type")]
        public string SecretType { get; set; }

        [JsonPropertyName("secret_type_display_name")]
        public string SecretTypeDisplayName { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("validity")]
        public string Validity { get; set; }

        [JsonPropertyName("multi_repo")]
        public bool MultiRepo { get; set; }

        [JsonPropertyName("is_base64_encoded")]
        public bool IsBase64Encoded { get; set; }

        [JsonPropertyName("publicly_leaked")]
        public bool PubliclyLeaked { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("resolved_by")]
        public object ResolvedBy { get; set; }

        [JsonPropertyName("resolved_at")]
        public object ResolvedAt { get; set; }

        [JsonPropertyName("resolution_comment")]
        public object ResolutionComment { get; set; }

        [JsonPropertyName("push_protection_bypassed")]
        public bool PushProtectionBypassed { get; set; }

        [JsonPropertyName("push_protection_bypassed_by")]
        public PushProtectionBypassedBy PushProtectionBypassedBy { get; set; }

        [JsonPropertyName("push_protection_bypassed_at")]
        public DateTime? PushProtectionBypassedAt { get; set; }

        [JsonPropertyName("push_protection_bypass_request_reviewer")]
        public object PushProtectionBypassRequestReviewer { get; set; }

        [JsonPropertyName("push_protection_bypass_request_reviewer_comment")]
        public object PushProtectionBypassRequestReviewerComment { get; set; }

        [JsonPropertyName("push_protection_bypass_request_comment")]
        public object PushProtectionBypassRequestComment { get; set; }

        [JsonPropertyName("push_protection_bypass_request_html_url")]
        public object PushProtectionBypassRequestHtmlUrl { get; set; }
    }

    public class PushProtectionBypassedBy
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("gravatar_id")]
        public string GravatarId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("followers_url")]
        public string FollowersUrl { get; set; }

        [JsonPropertyName("following_url")]
        public string FollowingUrl { get; set; }

        [JsonPropertyName("gists_url")]
        public string GistsUrl { get; set; }

        [JsonPropertyName("starred_url")]
        public string StarredUrl { get; set; }

        [JsonPropertyName("subscriptions_url")]
        public string SubscriptionsUrl { get; set; }

        [JsonPropertyName("organizations_url")]
        public string OrganizationsUrl { get; set; }

        [JsonPropertyName("repos_url")]
        public string ReposUrl { get; set; }

        [JsonPropertyName("events_url")]
        public string EventsUrl { get; set; }

        [JsonPropertyName("received_events_url")]
        public string ReceivedEventsUrl { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("user_view_type")]
        public string UserViewType { get; set; }

        [JsonPropertyName("site_admin")]
        public bool SiteAdmin { get; set; }
    }
}
