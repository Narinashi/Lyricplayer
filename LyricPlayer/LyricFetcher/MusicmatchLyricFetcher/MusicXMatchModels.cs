using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{

    internal partial class TrackLyricResponse
    {
        [JsonProperty("message")]
        public TrackLyricMessage Message { get; set; }
    }

    internal partial class TrackLyricMessage
    {
        [JsonProperty("header")]
        public StickyHeader Header { get; set; }

        [JsonProperty("body")]
        public PurpleBody Body { get; set; }
    }

    internal partial class PurpleBody
    {
        [JsonProperty("macro_calls")]
        public MacroCalls MacroCalls { get; set; }
    }

    internal partial class MacroCalls
    {
        [JsonProperty("track.lyrics.get")]
        public TrackLyricsGet TrackLyric { get; set; }

        [JsonProperty("track.subtitles.get")]
        public TrackSubtitlesGet TrackSubtitle { get; set; }

        [JsonProperty("track.richsync.get")]
        public TrackRichsyncGet TrackRichsync { get; set; }

        [JsonProperty("matcher.track.get")]
        public MatcherTrackGet MatcherTrackGet { get; set; }
    }
    public class MatcherTrackGet
    {
        [JsonProperty("message")]
        public MatcherTrackGetMessage Message { get; set; }
    }

    public class MatcherTrackGetMessage
    {
        [JsonProperty("body")]
        public FluffyBody Body { get; set; }
    }

    public class FluffyBody
    {
        [JsonProperty("track")]
        public TrackerTrack Track { get; set; }
    }

    public class TrackerTrack
    {
        [JsonProperty("track_id")]
        public long TrackId { get; set; }

        [JsonProperty("track_mbid")]
        public string TrackMbid { get; set; }

        [JsonProperty("track_isrc")]
        public string TrackIsrc { get; set; }

        [JsonProperty("commontrack_isrcs")]
        public List<List<string>> CommontrackIsrcs { get; set; }

        [JsonProperty("track_spotify_id")]
        public string TrackSpotifyId { get; set; }

        [JsonProperty("commontrack_spotify_ids")]
        public List<string> CommontrackSpotifyIds { get; set; }

        [JsonProperty("track_soundcloud_id")]
        public long TrackSoundcloudId { get; set; }

        [JsonProperty("track_xboxmusic_id")]
        public string TrackXboxmusicId { get; set; }

        [JsonProperty("track_name")]
        public string TrackName { get; set; }

        [JsonProperty("track_rating")]
        public long TrackRating { get; set; }

        [JsonProperty("track_length")]
        public long TrackLength { get; set; }

        [JsonProperty("commontrack_id")]
        public long CommontrackId { get; set; }

        [JsonProperty("instrumental")]
        public long Instrumental { get; set; }

        [JsonProperty("explicit")]
        public long Explicit { get; set; }

        [JsonProperty("has_lyrics")]
        public long HasLyrics { get; set; }

        [JsonProperty("has_lyrics_crowd")]
        public long HasLyricsCrowd { get; set; }

        [JsonProperty("has_subtitles")]
        public long HasSubtitles { get; set; }

        [JsonProperty("has_richsync")]
        public long HasRichsync { get; set; }

        [JsonProperty("has_track_structure")]
        public long HasTrackStructure { get; set; }

        [JsonProperty("num_favourite")]
        public long NumFavourite { get; set; }

        [JsonProperty("lyrics_id")]
        public long LyricsId { get; set; }

        [JsonProperty("subtitle_id")]
        public long SubtitleId { get; set; }

        [JsonProperty("album_id")]
        public long AlbumId { get; set; }

        [JsonProperty("album_name")]
        public string AlbumName { get; set; }

        [JsonProperty("artist_id")]
        public long ArtistId { get; set; }

        [JsonProperty("artist_mbid")]
        public string ArtistMbid { get; set; }

        [JsonProperty("artist_name")]
        public string ArtistName { get; set; }

        [JsonProperty("album_coverart_100x100")]
        public Uri AlbumCoverart100X100 { get; set; }

        [JsonProperty("album_coverart_350x350")]
        public Uri AlbumCoverart350X350 { get; set; }

        [JsonProperty("album_coverart_500x500")]
        public Uri AlbumCoverart500X500 { get; set; }

        [JsonProperty("album_coverart_800x800")]
        public string AlbumCoverart800X800 { get; set; }

        [JsonProperty("commontrack_vanity_id")]
        public string CommontrackVanityId { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }
    }

    internal partial class PurpleHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }
    }

    internal partial class TrackLyricsGet
    {
        [JsonProperty("message")]
        public TrackLyricsGetMessage Message { get; set; }
    }

    internal partial class TrackLyricsGetMessage
    {
        [JsonProperty("header")]
        public PurpleHeader Header { get; set; }

        [JsonProperty("body")]
        public TentacledBody Body { get; set; }
    }

    internal partial class TentacledBody
    {
        [JsonProperty("crowd_lyrics_list")]
        public List<object> CrowdLyricsList { get; set; }

        [JsonProperty("lyrics")]
        public Lyrics Lyrics { get; set; }
    }

    internal partial class Lyrics
    {
        [JsonProperty("lyrics_id")]
        public long LyricsId { get; set; }

        [JsonProperty("can_edit")]
        public long CanEdit { get; set; }

        [JsonProperty("locked")]
        public long Locked { get; set; }

        [JsonProperty("published_status")]
        public long PublishedStatus { get; set; }

        [JsonProperty("action_requested")]
        public string ActionRequested { get; set; }

        [JsonProperty("verified")]
        public long Verified { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("instrumental")]
        public long Instrumental { get; set; }

        [JsonProperty("explicit")]
        public long Explicit { get; set; }

        [JsonProperty("lyrics_body")]
        public string LyricsBody { get; set; }

        [JsonProperty("lyrics_language")]
        public string LyricsLanguage { get; set; }

        [JsonProperty("lyrics_language_description")]
        public string LyricsLanguageDescription { get; set; }

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; }

        [JsonProperty("pixel_tracking_url")]
        public string PixelTrackingUrl { get; set; }

        [JsonProperty("html_tracking_url")]
        public string HtmlTrackingUrl { get; set; }

        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright { get; set; }

        [JsonProperty("writer_list")]
        public List<object> WriterList { get; set; }

        [JsonProperty("publisher_list")]
        public List<object> PublisherList { get; set; }

        [JsonProperty("backlink_url")]
        public string BacklinkUrl { get; set; }

        [JsonProperty("updated_time")]
        public DateTimeOffset UpdatedTime { get; set; }

        [JsonProperty("lyrics_user")]
        public User LyricsUser { get; set; }

        [JsonProperty("lyrics_verified_by")]
        public List<object> LyricsVerifiedBy { get; set; }
    }

    internal partial class User
    {
        [JsonProperty("user")]
        public LyricsUserUser UserUser { get; set; }
    }

    internal partial class LyricsUserUser
    {
        [JsonProperty("uaid")]
        public string Uaid { get; set; }

        [JsonProperty("is_mine")]
        public long IsMine { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_profile_photo")]
        public string UserProfilePhoto { get; set; }

        [JsonProperty("has_private_profile")]
        public long HasPrivateProfile { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("weekly_score")]
        public long WeeklyScore { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("rank_level")]
        public long RankLevel { get; set; }

        [JsonProperty("points_to_next_level")]
        public long PointsToNextLevel { get; set; }

        [JsonProperty("ratio_to_next_level")]
        public double RatioToNextLevel { get; set; }

        [JsonProperty("rank_name")]
        public string RankName { get; set; }

        [JsonProperty("next_rank_name")]
        public string NextRankName { get; set; }

        [JsonProperty("ratio_to_next_rank")]
        public double RatioToNextRank { get; set; }

        [JsonProperty("rank_color")]
        public string RankColor { get; set; }

        [JsonProperty("rank_colors")]
        public RankColors RankColors { get; set; }

        [JsonProperty("rank_image_url")]
        public string RankImageUrl { get; set; }

        [JsonProperty("next_rank_color")]
        public string NextRankColor { get; set; }

        [JsonProperty("next_rank_colors")]
        public RankColors NextRankColors { get; set; }

        [JsonProperty("next_rank_image_url")]
        public string NextRankImageUrl { get; set; }

        [JsonProperty("counters")]
        public Dictionary<string, long> Counters { get; set; }

        [JsonProperty("moderator")]
        public bool Moderator { get; set; }

        [JsonProperty("academy_completed")]
        public bool AcademyCompleted { get; set; }
    }

    internal partial class RankColors
    {
        [JsonProperty("rank_color_10")]
        public string RankColor10 { get; set; }

        [JsonProperty("rank_color_50")]
        public string RankColor50 { get; set; }

        [JsonProperty("rank_color_100")]
        public string RankColor100 { get; set; }

        [JsonProperty("rank_color_200")]
        public string RankColor200 { get; set; }
    }

    internal partial class TrackRichsyncGet
    {
        [JsonProperty("message")]
        public TrackRichsyncGetMessage Message { get; set; }
    }

    internal partial class TrackRichsyncGetMessage
    {
        [JsonProperty("header")]
        public FluffyHeader Header { get; set; }

        [JsonProperty("body")]
        public StickyBody Body { get; set; }
    }

    internal partial class StickyBody
    {
        [JsonProperty("richsync")]
        public Richsync Richsync { get; set; }
    }

    internal partial class Richsync
    {
        [JsonProperty("richsync_id")]
        public long RichsyncId { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("richsync_body")]
        public string RichsyncBody { get; set; }

        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright { get; set; }

        [JsonProperty("richsync_length")]
        public long RichsyncLength { get; set; }

        [JsonProperty("richssync_language")]
        public string RichssyncLanguage { get; set; }

        [JsonProperty("richsync_language_description")]
        public string RichsyncLanguageDescription { get; set; }

        [JsonProperty("richsync_avg_count")]
        public long RichsyncAvgCount { get; set; }

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; }

        [JsonProperty("pixel_tracking_url")]
        public string PixelTrackingUrl { get; set; }

        [JsonProperty("html_tracking_url")]
        public string HtmlTrackingUrl { get; set; }

        [JsonProperty("writer_list")]
        public List<object> WriterList { get; set; }

        [JsonProperty("publisher_list")]
        public List<object> PublisherList { get; set; }

        [JsonProperty("updated_time")]
        public DateTimeOffset UpdatedTime { get; set; }

        [JsonProperty("richsync_user")]
        public RichsyncUser RichsyncUser { get; set; }
    }

    internal partial class RichsyncUser
    {
        [JsonProperty("user")]
        public RichsyncUserUser User { get; set; }
    }

    internal partial class RichsyncUserUser
    {
        [JsonProperty("uaid")]
        public string Uaid { get; set; }

        [JsonProperty("is_mine")]
        public long IsMine { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_profile_photo")]
        public string UserProfilePhoto { get; set; }

        [JsonProperty("has_private_profile")]
        public long HasPrivateProfile { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("weekly_score")]
        public long WeeklyScore { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("rank_level")]
        public long RankLevel { get; set; }

        [JsonProperty("points_to_next_level")]
        public long PointsToNextLevel { get; set; }

        [JsonProperty("ratio_to_next_level")]
        public double RatioToNextLevel { get; set; }

        [JsonProperty("rank_name")]
        public string RankName { get; set; }

        [JsonProperty("next_rank_name")]
        public string NextRankName { get; set; }

        [JsonProperty("ratio_to_next_rank")]
        public long RatioToNextRank { get; set; }

        [JsonProperty("rank_color")]
        public string RankColor { get; set; }

        [JsonProperty("rank_colors")]
        public RankColors RankColors { get; set; }

        [JsonProperty("rank_image_url")]
        public string RankImageUrl { get; set; }

        [JsonProperty("next_rank_color")]
        public string NextRankColor { get; set; }

        [JsonProperty("next_rank_colors")]
        public RankColors NextRankColors { get; set; }

        [JsonProperty("next_rank_image_url")]
        public string NextRankImageUrl { get; set; }

        [JsonProperty("counters")]
        public Dictionary<string, long> Counters { get; set; }

        [JsonProperty("academy_completed")]
        public bool AcademyCompleted { get; set; }
    }

    internal partial class FluffyHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("available")]
        public long Available { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }

        [JsonProperty("instrumental", NullValueHandling = NullValueHandling.Ignore)]
        public long? Instrumental { get; set; }
    }

    internal partial class TrackSubtitlesGet
    {
        [JsonProperty("message")]
        public TrackSubtitlesGetMessage Message { get; set; }
    }

    internal partial class TrackSubtitlesGetMessage
    {
        [JsonProperty("header")]
        public FluffyHeader Header { get; set; }

        [JsonProperty("body")]
        public IndecentBody Body { get; set; }
    }

    internal partial class IndecentBody
    {
        [JsonProperty("subtitle_list")]
        public List<SubtitleList> SubtitleList { get; set; }
    }

    internal partial class SubtitleList
    {
        [JsonProperty("subtitle")]
        public Subtitle Subtitle { get; set; }
    }

    internal partial class Subtitle
    {
        [JsonProperty("subtitle_id")]
        public long SubtitleId { get; set; }

        [JsonProperty("restricted")]
        public long Restricted { get; set; }

        [JsonProperty("subtitle_body")]
        public string SubtitleBody { get; set; }

        [JsonProperty("subtitle_avg_count")]
        public long SubtitleAvgCount { get; set; }

        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright { get; set; }

        [JsonProperty("subtitle_length")]
        public long SubtitleLength { get; set; }

        [JsonProperty("subtitle_language")]
        public string SubtitleLanguage { get; set; }

        [JsonProperty("subtitle_language_description")]
        public string SubtitleLanguageDescription { get; set; }

        [JsonProperty("script_tracking_url")]
        public string ScriptTrackingUrl { get; set; }

        [JsonProperty("pixel_tracking_url")]
        public string PixelTrackingUrl { get; set; }

        [JsonProperty("html_tracking_url")]
        public string HtmlTrackingUrl { get; set; }

        [JsonProperty("writer_list")]
        public List<object> WriterList { get; set; }

        [JsonProperty("publisher_list")]
        public List<object> PublisherList { get; set; }

        [JsonProperty("updated_time")]
        public DateTimeOffset UpdatedTime { get; set; }

        [JsonProperty("subtitle_user")]
        public User SubtitleUser { get; set; }
    }

    internal partial class StickyHeader
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("execute_time")]
        public double ExecuteTime { get; set; }

        [JsonProperty("pid")]
        public long Pid { get; set; }

        [JsonProperty("surrogate_key_list")]
        public List<object> SurrogateKeyList { get; set; }
    }

    public class TrackRichSyncLyric
    {
        [JsonProperty("ts")]
        public double StartAt { get; set; }

        [JsonProperty("te")]
        public double EndAt { get; set; }

        [JsonIgnore]
        public double Duration => EndAt - StartAt;

        [JsonProperty("l")]
        public List<Line> Lines { get; set; }

        [JsonProperty("x")]
        public string Lyric { get; set; }
    }

    public class Line
    {
        [JsonProperty("c")]
        public string Char { get; set; }

        [JsonProperty("o")]
        public double RelativeOffset { get; set; }
    }

    public class LyricSubtitle
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time")]
        public Time Time { get; set; }
    }

    public class Time
    {
        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("minutes")]
        public long Minutes { get; set; }

        [JsonProperty("seconds")]
        public long Seconds { get; set; }

        [JsonProperty("hundredths")]
        public long Hundredths { get; set; }
    }

}
