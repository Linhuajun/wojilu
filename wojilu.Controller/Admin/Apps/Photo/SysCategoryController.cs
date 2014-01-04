﻿/*
 * Copyright (c) 2010, www.wojilu.com. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;

using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Apps.Photo.Domain;
using wojilu.Apps.Photo.Service;
using wojilu.Apps.Photo.Interface;
using wojilu.Members.Users.Domain;
using wojilu.Members.Sites.Interface;
using wojilu.Members.Sites.Service;
using wojilu.Web.Controller.Security;
using wojilu.Members.Interface;
using wojilu.Members.Sites.Domain;
using wojilu.Common.AppBase.Interface;
using wojilu.Common.AppBase;

namespace wojilu.Web.Controller.Admin.Apps.Photo {

    [App( typeof( PhotoApp ) )]
    public class SysCategoryController : ControllerBase {

        public virtual IPhotoSysCategoryService categoryService { get; set; }
        public virtual IAdminLogService<SiteLog> logService { get; set; }

        public SysCategoryController() {
            categoryService = new PhotoSysCategoryService();
            logService = new SiteLogService();
        }

        private void log( String msg, PhotoSysCategory c ) {
            String dataInfo = "{Id:" + c.Id + ", Name:'" + c.Name + "'}";
            logService.Add( (User)ctx.viewer.obj, msg, dataInfo, typeof( PhotoSysCategory ).FullName, ctx.Ip );
        }

        public virtual void List() {
            target( Add );
            List<PhotoSysCategory> categories = categoryService.GetAll();
            bindList( "list", "category", categories, bindLink );
            set( "sortAction", to( SaveSort ) );
        }

        private void bindLink( IBlock tpl, long id ) {
            tpl.Set( "category.LinkEdit", to( Edit, id ) );
            tpl.Set( "category.LinkDelete", to( Delete, id ) );
        }



        [HttpPost, DbTransaction]
        public virtual void SaveSort() {

            long id = ctx.PostLong( "id" );
            String cmd = ctx.Post( "cmd" );

            PhotoSysCategory target = categoryService.GetById( id );
            List<PhotoSysCategory> list = categoryService.GetAll();

            if (cmd == "up") {

                new SortUtil<PhotoSysCategory>( target, list ).MoveUp();
                echoJsonOk();
            }
            else if (cmd == "down") {

                new SortUtil<PhotoSysCategory>( target, list ).MoveDown();
                echoJsonOk();
            }
            else {
                echoError( lang( "exUnknowCmd" ) );
            }

        }

        public virtual void Add() {
            target( Create );
        }

        [HttpPost, DbTransaction]
        public virtual void Create() {

            PhotoSysCategory c = validate( null );
            if (ctx.HasErrors) {
                run( Add );
                return;
            }

            db.insert( c );
            log( SiteLogString.InsertPhotoSysCategory(), c );

            echoToParentPart( lang("opok") );
        }

        public virtual void Edit( long id ) {

            target( Update, id );

            PhotoSysCategory c = categoryService.GetById( id );
            if (c == null) {
                echoRedirect( lang( "exDataNotFound" ) );
                return;
            }

            bind( "category", c );
        }

        [HttpPost, DbTransaction]
        public virtual void Update( long id ) {

            PhotoSysCategory c = categoryService.GetById( id );
            if (c == null) {
                echoRedirect( lang( "exDataNotFound" ) );
                return;
            }

            c = validate( c );
            if (ctx.HasErrors) {
                run( Edit, id );
                return;
            }

            db.update( c );
            log( SiteLogString.UpdatePhotoSysCategory(), c );

            echoToParentPart( lang( "opok" ) );
        }

        [HttpDelete, DbTransaction]
        public virtual void Delete( long id ) {

            PhotoSysCategory c = categoryService.GetById( id );
            if (c == null) {
                echoRedirect( lang( "exDataNotFound" ) );
                return;
            }
            db.delete( c );
            log( SiteLogString.DeletePhotoSysCategory(), c );

            redirect( List );
        }

        private PhotoSysCategory validate( PhotoSysCategory c ) {
            if (c == null) c = new PhotoSysCategory();
            c = ctx.PostValue( c ) as PhotoSysCategory;
            if (strUtil.IsNullOrEmpty( c.Name )) errors.Add( lang( "exName" ) );
            return c;
        }

    }
}
