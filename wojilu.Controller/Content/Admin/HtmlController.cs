﻿using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Apps.Content.Domain;
using wojilu.Apps.Content.Interface;
using wojilu.Apps.Content.Service;
using wojilu.Web.Controller.Content.Caching;
using wojilu.Serialization;
using wojilu.Members.Sites.Domain;

namespace wojilu.Web.Controller.Content.Admin {

    [App( typeof( ContentApp ) )]
    public class HtmlController : ControllerBase {

        private static readonly ILog logger = LogManager.GetLogger( typeof( HtmlController ) );

        public IContentPostService postService { get; set; }
        public IContentSectionService sectionService { get; set; }

        public HtmlController() {
            postService = new ContentPostService();
            sectionService = new ContentSectionService();
        }

        public override void CheckPermission() {
            if (ctx.owner.obj is Site == false) {
                echoError( "没有权限，目前只支持网站生成html页面" );
            }                
        }

        public void Index() {

            bindHtmlDir();

            set( "lnkMakeAll", to( MakeAll ) );
            set( "lnkMakeDetailAll", to( MakeDetailAll ) );
            set( "lnkMakeSectionAll", to( MakeSectionAll ) );
            set( "lnkMakeHome", to( MakeHomePage ) );
            set( "lnkMakeSidebar", to( MakeSidebar ) );

            IBlock sectionBlock = getBlock( "sections" );
            List<ContentSection> sections = sectionService.GetByApp( ctx.app.Id );
            foreach (ContentSection section in sections) {
                sectionBlock.Set( "section.Name", section.Title );
                sectionBlock.Set( "lnkMakeSection", to( MakeSection, section.Id ) );
                sectionBlock.Set( "lnkMakeDetail", to( MakeDetailBySection, section.Id ) );
                sectionBlock.Next();
            }

        }

        private void bindHtmlDir() {
            ContentApp app = ctx.app.obj as ContentApp;
            ContentSetting s = app.GetSettingsObj();

            String htmlDir = HtmlHelper.GetlAppStaticDir( app.Id );
            htmlDir = htmlDir.TrimStart( '/' ).TrimEnd( '/' );

            set( "htmlDir", htmlDir );

            set( "host", ctx.url.SiteAndAppPath );
            set( "editHtmlDirLink", to( EditHtmlDir ) );

        }

        public void EditHtmlDir() {

            target( SaveHtmlDir );

            ContentApp app = ctx.app.obj as ContentApp;
            ContentSetting s = app.GetSettingsObj();

            String htmlDir = HtmlHelper.GetlAppStaticDir( app.Id );
            htmlDir = htmlDir.TrimStart( '/' ).TrimEnd( '/' );

            set( "htmlDir", htmlDir );

            set( "host", ctx.url.SiteAndAppPath );

        }

        public void SaveHtmlDir() {

            String htmlDir = strUtil.SubString( ctx.Post( "htmlDir" ), 30 );
            if (strUtil.IsNullOrEmpty( htmlDir )) {
                echoError( "请填写内容" );
                return;
            }

            if (HtmlHelper.IsHtmlDirError( htmlDir, ctx.errors )) {
                echoError();
                return;
            }

            ContentApp app = ctx.app.obj as ContentApp;
            ContentSetting s = app.GetSettingsObj();
            s.StaticDir = htmlDir;

            app.Settings = JsonString.ConvertObject( s );
            app.update();

            echoToParentPart( lang( "opok" ) );

        }

        private int htmlCount = 0;

        public void MakeAll() {
            view( "MakeDone" );

            MakeSectionAll();
            MakeDetailAll();
            MakeHomePage();
            MakeSidebar();

            echo( "生成所有静态页面成功，共 " + htmlCount + " 篇" );
        }

        public void MakeSidebar() {
            view( "MakeDone" );
            HtmlHelper.MakeSidebarHtml( ctx );
            echo( "生成侧栏页成功" );

            htmlCount += 1;
        }

        public void MakeHomePage() {
            HtmlHelper.MakeAppHtml( ctx );
            echo( "生成首页成功" );
        }

        public void MakeSectionAll() {

            view( "MakeDone" );

            ContentApp app = ctx.app.obj as ContentApp;

            int count = 0;
            List<ContentSection> sections = sectionService.GetByApp( ctx.app.Id );
            foreach (ContentSection section in sections) {

                int recordCount = postService.GetCountBySection( section.Id );

                count += HtmlHelper.MakeListHtml( ctx, app, section.Id, recordCount );
                logger.Info( "make section html, sectionId=" + section.Id );
            }

            htmlCount += count;
            echo( "生成所有列表页成功，共 " + htmlCount + " 篇" );
        }

        public void MakeDetailAll() {

            view( "MakeDone" );


            List<ContentPost> list = postService.GetByApp( ctx.app.Id );
            makeDetail( list );

            htmlCount += list.Count;
        }


        public void MakeSection( int sectionId ) {

            view( "MakeDone" );

            ContentApp app = ctx.app.obj as ContentApp;
            int recordCount = postService.GetCountBySection( sectionId );

            int listCount = HtmlHelper.MakeListHtml( ctx, app, sectionId, recordCount );
            echo( "生成列表页成功，共 " + listCount + " 篇" );

        }

        public void MakeDetailBySection( int sectionId ) {

            view( "MakeDone" );

            List<ContentPost> list = postService.GetBySection( sectionId );
            echo( "生成详细页成功，共 " + list.Count + " 篇" );
        }


        //-------------------------------------------------------------------------------------------

        private void makeDetail( List<ContentPost> list ) {
            foreach (ContentPost post in list) {
                ctx.SetItem( "_currentContentPost", post );
                HtmlHelper.MakeDetailHtml( ctx );
                logger.Info( "make detail html, postId=" + post.Id );
            }

            echo( "生成所有详细页成功，共 " + list.Count + " 篇" );

        }


    }

}
