/*
 * Copyright (c) 2010, www.wojilu.com. All rights reserved.
 */

using System;
using System.Collections.Generic;

using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;

using wojilu.Apps.Content.Domain;
using wojilu.Apps.Content.Interface;
using wojilu.Apps.Content.Service;
using wojilu.Apps.Content.Enum;
using wojilu.Common.AppBase.Interface;
using wojilu.Common.AppBase;
using wojilu.Web.Controller.Content.Caching;

namespace wojilu.Web.Controller.Content.Admin.Section {


    [App( typeof( ContentApp ) )]
    public partial class TextController : ControllerBase, IPageSection {

        public IContentPostService postService { get; set; }
        public IContentSectionService sectionService { get; set; }

        public TextController() {
            postService = new ContentPostService();
            sectionService = new ContentSectionService();
        }

        public List<IPageSettingLink> GetSettingLink( int sectionId ) {
            List<IPageSettingLink> links = new List<IPageSettingLink>();

            PageSettingLink lnk = new PageSettingLink();
            lnk.Name = lang( "editSetting" );
            lnk.Url = to( new SectionSettingController().Edit, sectionId );
            links.Add( lnk );

            PageSettingLink lnktmp = new PageSettingLink();
            lnktmp.Name = alang( "editTemplate" );
            lnktmp.Url = to( new TemplateCustomController().Edit, sectionId );
            links.Add( lnktmp );

            return links;
        }

        public void SectionShow( int sectionId ) {
        }

        public void AdminSectionShow( int sectionId ) {
            ContentSection section = sectionService.GetById( sectionId, ctx.app.Id );
            ContentPost textPost = postService.GetFirstPost( ctx.app.Id, sectionId );

            bindSectionShow( sectionId, textPost );
        }

        public void AdminList( int sectionId ) {
            ContentSection section = sectionService.GetById( sectionId, ctx.app.Id );
            DataPage<ContentPost> posts = postService.GetBySectionAndCategory( section.Id, ctx.GetInt( "categoryId" ) );
            bindAdminList( section, posts );
        }

        public void Add( int sectionId ) {
            view( "Add" );
            ContentSection section = sectionService.GetById( sectionId, ctx.app.Id );
            target( Create, sectionId );
            bindAddInfo( section );
        }

        [HttpPost, DbTransaction]
        public void Create( int sectionId ) {

            ContentSection section = sectionService.GetById( sectionId, ctx.app.Id );

            ContentPost post = ContentValidator.SetValueBySection( sectionService.GetById( sectionId, ctx.app.Id ), ctx );
            if (strUtil.IsNullOrEmpty( post.Title )) {
                post.Title = section.Title + " " + DateTime.Now.ToShortDateString();
            }

            if (strUtil.IsNullOrEmpty( post.Content )) {
                errors.Add( lang( "exContent" ) );
                run( Add, sectionId );
            }
            else {
                post.CategoryId = PostCategory.Notice;

                postService.Insert( post, null );
               
                echoToParentPart( lang( "opok" ) );
                HtmlHelper.SetCurrentPost( ctx, post );
            }
        }

        [HttpDelete, DbTransaction]
        public void Delete( int postId ) {
            ContentPost post = postService.GetById( postId, ctx.owner.Id );
            if (post == null) {
                echo( lang( "exDataNotFound" ) );
                return;
            }

            postService.Delete( post );
            echoRedirect( lang( "opok" ) );
            HtmlHelper.SetCurrentPost( ctx, post );
        }

        public void Edit( int postId ) {
            view( "Edit" );
            ContentPost post = postService.GetById( postId, ctx.owner.Id );
            if (post == null) {
                echo( lang( "exDataNotFound" ) );
                return;
            }

            target( Update, postId );
            bindEditInfo( post );
        }

        [HttpPost, DbTransaction]
        public void Update( int postId ) {
            ContentPost post = postService.GetById( postId, ctx.owner.Id );
            if (post == null) {
                echo( lang( "exDataNotFound" ) );
                return;
            }

            ContentValidator.SetPostValue( post, ctx );
            if (strUtil.IsNullOrEmpty( post.Content )) {
                errors.Add( lang( "exContent" ) );
                run( Edit, postId );
            }
            else {
                postService.Update( post, null );

                echoToParentPart( lang( "opok" ) );
                HtmlHelper.SetCurrentPost( ctx, post );
            }
        }


    }
}

