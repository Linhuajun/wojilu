/*
 * Copyright 2010 www.wojilu.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Context;
using wojilu.Caching;

namespace wojilu.Web.Mvc {

    /// <summary>
    /// action 缓存基类，包括缓存key，更新操作等
    /// </summary>
    public class ActionCache : ActionObserver {

        /// <summary>
        /// 设置被缓存的 action 的缓存 key。如果返回 null ，则不会被缓存。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public virtual string GetCacheKey( MvcContext ctx, String actionName ) {
            return null;
        }

    }
}
