﻿namespace SpiritualNetwork.API.Model
{
    public class BlockUnBlockReq
    {
        public int PostId { get; set; }
    }

    public class DeleteUpdateProfileSuggestion
    {
        public int Id { get; set;}
    }

    public class SearchProfileSuggestion
    {
        public string Name { get; set; }
        public string Type { get; set; }

    }

    public class BookMarkRes
    {
        public int Id { get; set; }
        public string Type { get; set; }

    }

    public class SuggestRes
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }

    }

    public class NotificationReq
    {
        public int PageNo { get; set; }
        public int Size { get; set; }
    }

}
