namespace RestFoundation
{
    /// <summary>
    /// Defines most commonly used link "rel" element values.
    /// </summary>
    public static class LinkRelationValues
    {
        /// <summary>
        /// Refers to a resource that is the subject of the link's context.
        /// </summary>
        public const string About = "about";

        /// <summary>
        /// Refers to a substitute for this context.
        /// </summary>
        public const string Alternate = "alternate";

        /// <summary>
        /// Refers to an appendix.
        /// </summary>
        public const string Appendix = "appendix";

        /// <summary>
        /// Refers to a collection of records, documents, or other materials of historical interest.
        /// </summary>
        public const string Archives = "archives";

        /// <summary>
        /// Refers to the context's author.
        /// </summary>
        public const string Author = "author";

        /// <summary>
        /// Gives a permanent link to use for bookmarking purposes.
        /// </summary>
        public const string Bookmark = "bookmark";

        /// <summary>
        /// Designates the preferred version of a resource (the URI and its contents).
        /// </summary>
        public const string Canonical = "canonical";

        /// <summary>
        /// The target URI points to a resource which represents the collection resource for the context URI.
        /// </summary>
        public const string Collection = "collection";

        /// <summary>
        /// Refers to a table of contents.
        /// </summary>
        public const string Contents = "contents";

        /// <summary>
        /// Refers to a copyright statement that applies to the link's context.
        /// </summary>
        public const string Copyright = "copyright";

        /// <summary>
        /// Refers to a resource containing the most recent item(s) in a collection of resources.
        /// </summary>
        public const string Current = "current";

        /// <summary>
        /// Refers to a resource providing information about the link's context.
        /// </summary>
        public const string DescribedBy = "describedby";

        /// <summary>
        /// The relationship A 'describes' B asserts that resource A provides a description of resource B.
        /// There are no constraints on the format or representation of either A or B, neither are there any
        /// further constraints on either resource.
        /// This link relation type is intended to be the inverse of the already existing 'describedby' relation
        /// type, thus allowing links to be established from the describing resource to the described resource.
        /// </summary>
        public const string Describes = "describes";

        /// <summary>
        /// Refers to a resource that can be used to edit the link's context.
        /// </summary>
        public const string Edit = "edit";

        /// <summary>
        /// An URI that refers to the furthest preceding resource in a series of resources.
        /// </summary>
        public const string First = "first";

        /// <summary>
        /// Refers to a glossary of terms.
        /// </summary>
        public const string Glossary = "glossary";

        /// <summary>
        /// Refers to context-sensitive help.
        /// </summary>
        public const string Help = "help";

        /// <summary>
        /// Refers to an icon representing the link's context.
        /// </summary>
        public const string Icon = "icon";

        /// <summary>
        /// Refers to an index.
        /// </summary>
        public const string Index = "index";

        /// <summary>
        /// The target URI points to a resource that is a member of the collection represented by the context URI.
        /// </summary>
        public const string Item = "item";

        /// <summary>
        /// An IRI that refers to the furthest following resource in a series of resources.
        /// </summary>
        public const string Last = "last";

        /// <summary>
        /// Points to a resource containing the latest (e.g., current) version of the context.
        /// </summary>
        public const string LatestVersion = "latest-version";

        /// <summary>
        /// Refers to a license associated with this context.
        /// </summary>
        public const string License = "license";

        /// <summary>
        /// Refers to a resource that can be used to monitor changes in an HTTP resource.
        /// </summary>
        public const string Monitor = "monitor";

        /// <summary>
        /// Indicates that the link's context is a part of a series, and that the next in the series is the link target.
        /// </summary>
        public const string Next = "next";

        /// <summary>
        /// Refers to the immediately following archive resource.
        /// </summary>
        public const string NextArchive = "next-archive";

        /// <summary>
        /// Indicates that the context’s original author or publisher does not endorse the link target.
        /// </summary>
        public const string NoFollow = "nofollow";

        /// <summary>
        /// Indicates that no referrer information is to be leaked when following the link.
        /// </summary>
        public const string NoReferrer = "noreferrer";

        /// <summary>
        /// Indicates a resource where payment is accepted.
        /// </summary>
        public const string Payment = "payment";

        /// <summary>
        /// Indicates that the link target should be preemptively cached.
        /// </summary>
        public const string PreFetch = "prefetch";

        /// <summary>
        /// Refers to a resource that provides a preview of the link's context.
        /// </summary>
        public const string Preview = "preview";

        /// <summary>
        /// Indicates that the link's context is a part of a series, and that the previous in the series is the link target.
        /// </summary>
        public const string Previous = "prev";

        /// <summary>
        /// Refers to the immediately preceding archive resource.
        /// </summary>
        public const string PreviousArchive = "prev-archive";

        /// <summary>
        /// Refers to a Privacy Policy associated with the link's context.
        /// </summary>
        public const string PrivacyPolicy = "privacy-policy";

        /// <summary>
        /// Identifying that a resource representation conforms to a certain profile, without affecting the non-profile
        /// semantics of the resource representation.
        /// </summary>
        public const string Profile = "profile";

        /// <summary>
        /// Identifies a related resource.
        /// </summary>
        public const string Related = "related";

        /// <summary>
        /// Refers to a resource that can be used to search through the link's context and related resources.
        /// </summary>
        public const string Search = "search";

        /// <summary>
        /// Refers to a section in a collection of resources.
        /// </summary>
        public const string Section = "section";

        /// <summary>
        /// Conveys an identifier for the link's context.
        /// </summary>
        public const string Self = "self";

        /// <summary>
        /// Indicates a URI that can be used to retrieve a service document.
        /// </summary>
        public const string Service = "service";

        /// <summary>
        /// Refers to the first resource in a collection of resources.
        /// </summary>
        public const string Start = "start";

        /// <summary>
        /// Refers to a stylesheet.
        /// </summary>
        public const string StyleSheet = "stylesheet";

        /// <summary>
        /// Refers to a resource serving as a subsection in a collection of resources.
        /// </summary>
        public const string SubSection = "subsection";

        /// <summary>
        /// Gives a tag (identified by the given address) that applies to the current document.   
        /// </summary>
        public const string Tag = "tag";

        /// <summary>
        /// Refers to the Terms of Service associated with the link's context.
        /// </summary>
        public const string TermsOfService = "terms-of-service";

        /// <summary>
        /// Refers to a resource identifying the abstract semantic type the link's context is considered to be an instance of.
        /// </summary>
        public const string Type = "type";

        /// <summary>
        /// Refers to a parent document in a hierarchy of documents.   
        /// </summary>
        public const string Up = "up";

        /// <summary>
        /// Identifies a resource that is the source of the information in the link's context.   
        /// </summary>
        public const string Via = "via";
    }
}
