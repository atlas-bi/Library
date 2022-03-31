using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Atlas_Web.Pages.Users
{
    [ResponseCache(NoStore = true)]
    public class FavoritesModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;

        public FavoritesModel(Atlas_WebContext context, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _config = config;
            _cache = cache;
        }

        public class FavoiteFolderRank
        {
            public string FolderId { get; set; }
            public int FolderRank { get; set; }
        }

        public class FavoiteRank
        {
            public string FavoriteId { get; set; }
            public string FavoriteType { get; set; }
            public int FavoriteRank { get; set; }
        }

        public List<AdList> AdLists { get; set; }

        public int UserId { get; set; }

        [BindProperty]
        public UserFavoriteFolder Folder { get; set; }

        public List<UserFavoriteFolder> Folders { get; set; }

        public List<StarredCollection> Collections { get; set; }
        public List<StarredInitiative> Initiatives { get; set; }
        public List<StarredTerm> Terms { get; set; }
        public List<StarredReport> Reports { get; set; }
        public List<StarredUser> Users { get; set; }
        public List<StarredGroup> Groups { get; set; }
        public List<StarredSearch> Searches { get; set; }

        public async Task<ActionResult> OnGet(int? id)
        {
            UserId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            // can user view others?
            var checkpoint = UserHelpers.CheckUserPermissions(
                _cache,
                _context,
                User.Identity.Name,
                37
            );

            if (checkpoint)
            {
                UserId = id ?? UserId;
            }

            Folders = await _context.UserFavoriteFolders
                .Where(x => x.UserId == UserId)
                .ToListAsync();

            Collections = await _context.StarredCollections
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.Collection)
                .ThenInclude(x => x.StarredCollections)
                .ToListAsync();

            Reports = await _context.StarredReports
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectDoc)
                .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectType)
                .Include(x => x.Report)
                .ThenInclude(x => x.ReportObjectAttachments)
                .Include(x => x.Report)
                .ThenInclude(x => x.StarredReports)
                .ToListAsync();

            Initiatives = await _context.StarredInitiatives
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.Initiative)
                .ThenInclude(x => x.StarredInitiatives)
                .ToListAsync();

            Terms = await _context.StarredTerms
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.Term)
                .ThenInclude(x => x.StarredTerms)
                .ToListAsync();

            Groups = await _context.StarredGroups
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.Group)
                .ThenInclude(x => x.StarredGroups)
                .ToListAsync();

            Searches = await _context.StarredSearches.Where(x => x.Ownerid == UserId).ToListAsync();

            Users = await _context.StarredUsers
                .Where(x => x.Ownerid == UserId)
                .Include(x => x.User)
                .ToListAsync();

            AdLists = new List<AdList>
            {
                new AdList { Url = "/Users?handler=SharedObjects", Column = 2 },
            };
            ViewData["AdLists"] = AdLists;

            return Page();
        }

        public ActionResult OnGetEdit(string type, int id, string search)
        {
            var MyId = UserHelpers.GetUser(_cache, _context, User.Identity.Name).UserId;

            if (type == "report")
            {
                if (_context.StarredReports.Any(x => x.Ownerid == MyId && x.Reportid == id))
                {
                    _context.RemoveRange(
                        _context.StarredReports.Where(x => x.Ownerid == MyId && x.Reportid == id)
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredReport { Ownerid = UserId, Reportid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("report-" + id);
                return Content(
                    _context.StarredReports.Where(x => x.Reportid == id).Count().ToString()
                );
            }
            else if (type == "collection")
            {
                if (_context.StarredCollections.Any(x => x.Ownerid == MyId && x.Collectionid == id))
                {
                    _context.RemoveRange(
                        _context.StarredCollections.Where(
                            x => x.Ownerid == MyId && x.Collectionid == id
                        )
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredCollection { Ownerid = MyId, Collectionid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("collection-" + id);
                _cache.Remove("collections");
                return Content(
                    _context.StarredCollections.Where(x => x.Collectionid == id).Count().ToString()
                );
            }
            else if (type == "initiative")
            {
                if (_context.StarredInitiatives.Any(x => x.Ownerid == MyId && x.Initiativeid == id))
                {
                    _context.RemoveRange(
                        _context.StarredInitiatives.Where(
                            x => x.Ownerid == MyId && x.Initiativeid == id
                        )
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredInitiative { Ownerid = UserId, Initiativeid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("initiative-" + id);
                _cache.Remove("initatives");
                return Content(
                    _context.StarredInitiatives.Where(x => x.Initiativeid == id).Count().ToString()
                );
            }
            else if (type == "term")
            {
                if (_context.StarredTerms.Any(x => x.Ownerid == MyId && x.Termid == id))
                {
                    _context.RemoveRange(
                        _context.StarredTerms.Where(x => x.Ownerid == MyId && x.Termid == id)
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredTerm { Ownerid = MyId, Termid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("term-" + id);
                _cache.Remove("terms");
                return Content(_context.StarredTerms.Where(x => x.Termid == id).Count().ToString());
            }
            else if (type == "user")
            {
                if (_context.StarredUsers.Any(x => x.Ownerid == MyId && x.Userid == id))
                {
                    _context.RemoveRange(
                        _context.StarredUsers.Where(x => x.Ownerid == MyId && x.Userid == id)
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredUser { Ownerid = MyId, Userid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("user-" + id);
                return Content(_context.StarredUsers.Where(x => x.Userid == id).Count().ToString());
            }
            else if (type == "group")
            {
                if (_context.StarredGroups.Any(x => x.Ownerid == MyId && x.Groupid == id))
                {
                    _context.RemoveRange(
                        _context.StarredGroups.Where(x => x.Ownerid == MyId && x.Groupid == id)
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredGroup { Ownerid = MyId, Groupid = id });
                    _context.SaveChanges();
                }
                // pop from cache
                _cache.Remove("group-" + id);
                return Content(
                    _context.StarredGroups.Where(x => x.Groupid == id).Count().ToString()
                );
            }
            else if (type == "search")
            {
                if (_context.StarredSearches.Any(x => x.Ownerid == MyId && x.Search == search))
                {
                    _context.RemoveRange(
                        _context.StarredSearches.Where(
                            x => x.Ownerid == UserId && x.Search == search
                        )
                    );
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(new StarredSearch { Ownerid = MyId, Search = search });
                    _context.SaveChanges();
                }

                return Content(
                    _context.StarredSearches.Where(x => x.Search == search).Count().ToString()
                );
            }
            return Content("error");
        }

        public ActionResult OnPostEditFavorites(
            int actionType,
            int objectId,
            string favoriteType,
            string objectName
        )
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            if (actionType == 1)
            {
                var type = favoriteType;
                if (favoriteType.EndsWith("s"))
                {
                    type = favoriteType.Remove(favoriteType.Length - 1);
                }

                _context.Add(
                    new UserFavorite
                    {
                        UserId = MyUser.UserId,
                        ItemRank = -1,
                        ItemId = objectId,
                        ItemType = type,
                        ItemName = objectName == "null" ? null : objectName
                    }
                );
            }
            else if (actionType == 0)
            {
                if (favoriteType == "search")
                {
                    _context.RemoveRange(
                        _context.UserFavorites.Where(
                            x =>
                                x.UserId == MyUser.UserId
                                && x.ItemId == objectId
                                && x.ItemType == favoriteType
                        )
                    );
                }
                else
                {
                    _context.RemoveRange(
                        _context.UserFavorites.Where(
                            x => x.ItemId == objectId && x.ItemType == favoriteType
                        )
                    );
                }
            }

            _context.SaveChanges();

            return Content("ok");
        }

        public ActionResult OnPostNewFolder(string name)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            _context.Add(new UserFavoriteFolder { UserId = MyUser.UserId, FolderName = name });
            _context.SaveChanges();

            return Content("ok");
        }

        public ActionResult OnPostEditFolder(int id, string name)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            var folder = _context.UserFavoriteFolders.SingleOrDefault(
                x => x.UserFavoriteFolderId == id && x.UserId == MyUser.UserId
            );

            if (folder != null)
            {
                folder.FolderName = name;
            }
            _context.SaveChanges();

            return Content("ok");
        }

        public ActionResult OnPostDeleteFolder(int id)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            _context.StarredCollections
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredReports
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredInitiatives
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredTerms
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredUsers
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredGroups
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.StarredSearches
                .Where(x => x.Folderid == id && x.Ownerid == MyUser.UserId)
                .ToList()
                .ForEach(x => x.Folderid = null);
            _context.SaveChanges();

            _context.Remove(
                _context.UserFavoriteFolders.SingleOrDefault(x => x.UserFavoriteFolderId == id)
            );
            _context.SaveChanges();

            return Content("ok");
        }

        public ActionResult OnPostReorderFavorites([FromBody] dynamic package)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);

            foreach (var l in JsonSerializer.Deserialize<List<FavoiteRank>>(package))
            {
                int id = Int32.Parse(l.FavoriteId);
                if (l.FavoriteType == "report")
                {
                    _context.StarredReports.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "collection")
                {
                    _context.StarredCollections.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "initiative")
                {
                    _context.StarredInitiatives.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "term")
                {
                    _context.StarredTerms.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "user")
                {
                    _context.StarredUsers.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "group")
                {
                    _context.StarredGroups.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
                else if (l.FavoriteType == "search")
                {
                    _context.StarredSearches.SingleOrDefault(
                        x => x.StarId == id && x.Ownerid == MyUser.UserId
                    ).Rank = l.FavoriteRank;
                }
            }
            _context.SaveChanges();

            return Content("ok");
        }

        public async Task<ActionResult> OnPostUpdateFavoriteFolder()
        {
            var body = await new System.IO.StreamReader(Request.Body)
                .ReadToEndAsync()
                .ConfigureAwait(false);
            var package = JObject.Parse(body);

            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            int FavoriteId = (int)package["FavoriteId"];
            int? FolderId = (int)package["FolderId"];
            if (FolderId == 0)
            {
                FolderId = null;
            }
            string FavoriteType = (string)package["FavoriteType"];

            if (FavoriteType == "report")
            {
                _context.StarredReports.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "collection")
            {
                _context.StarredCollections.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "initiative")
            {
                _context.StarredInitiatives.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "term")
            {
                _context.StarredTerms.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "user")
            {
                _context.StarredUsers.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "group")
            {
                _context.StarredGroups.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }
            else if (FavoriteType == "search")
            {
                _context.StarredSearches.SingleOrDefault(
                    x => x.StarId == FavoriteId && x.Ownerid == MyUser.UserId
                ).Folderid = FolderId;
            }

            _context.SaveChanges();

            return Content("ok");
        }

        public ActionResult OnPostReorderFolders([FromBody] dynamic package)
        {
            var MyUser = UserHelpers.GetUser(_cache, _context, User.Identity.Name);
            foreach (var l in JsonSerializer.Deserialize<List<FavoiteFolderRank>>(package))
            {
                int id = Int32.Parse(l.FolderId);
                _context.UserFavoriteFolders
                    .Where(x => x.UserFavoriteFolderId == id && x.UserId == MyUser.UserId)
                    .FirstOrDefault().FolderRank = l.FolderRank;
            }
            _context.SaveChanges();

            return Content("ok");
        }
    }
}
