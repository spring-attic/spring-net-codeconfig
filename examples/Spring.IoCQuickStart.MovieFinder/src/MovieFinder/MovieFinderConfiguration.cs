#region License

/*
 * Copyright 2002-2010 the original author or authors.
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

#endregion

using System;
using System.IO;
using Spring.Context.Attributes;

namespace Spring.IocQuickStart.MovieFinder
{
    /// <summary>
    /// Configuration class for the MovieFinder application
    /// </summary>
    /// <author>Mark Pollack</author>
    [Configuration]
    public class MovieFinderConfiguration
    {

        [Definition]
        public virtual MovieLister MyMovieLister()
        {
            MovieLister movieLister =  new MovieLister();
            movieLister.MovieFinder = FileBasedMovieFinder();
            return movieLister;

        }

        [Definition]
        public virtual IMovieFinder FileBasedMovieFinder()
        {
            return new ColonDelimitedMovieFinder(new FileInfo("movies.txt"));
        }
    }

}