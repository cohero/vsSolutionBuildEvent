﻿/* 
 * Boost Software License - Version 1.0 - August 17th, 2003
 * 
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person or organization
 * obtaining a copy of the software and accompanying documentation covered by
 * this license (the "Software") to use, reproduce, display, distribute,
 * execute, and transmit the Software, and to prepare derivative works of the
 * Software, and to permit third-parties to whom the Software is furnished to
 * do so, all subject to the following:
 * 
 * The copyright notices in the Software and this entire statement, including
 * the above license grant, this restriction and the following disclaimer,
 * must be included in all copies of the Software, in whole or in part, and
 * all derivative works of the Software, unless such copies or derivative
 * works are solely in the form of machine-executable object code generated by
 * a source language processor.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
 * SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
 * FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reg.ext.vsSolutionBuildEvent
{
    [Serializable]
    public class SolutionEvents
    {
        private SBESettings _settings = new SBESettings();
        /// <summary>
        /// global settings
        /// </summary>
        public SBESettings settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        private SBEEvent _preBuild = new SBEEvent();
        /// <summary>
        /// Before building solution
        /// </summary>
        public SBEEvent preBuild
        {
            get { return _preBuild; }
            set { _preBuild = value; }
        }

        private SBEEvent _postBuild = new SBEEvent();
        /// <summary>
        /// After building solution
        /// </summary>
        public SBEEvent postBuild
        {
            get { return _postBuild; }
            set { _postBuild = value; }
        }

        private SBEEvent _cancelBuild = new SBEEvent();
        /// <summary>
        /// When cancel building solution
        /// e.g. fatal error of compilation or cancel of user
        /// </summary>
        public SBEEvent cancelBuild
        {
            get { return _cancelBuild; }
            set { _cancelBuild = value; }
        }

        private SBEEventEW _warningBuild = new SBEEventEW();
        /// <summary>
        /// Warnings during assembly
        /// </summary>
        public SBEEventEW warningBuild
        {
            get { return _warningBuild; }
            set { _warningBuild = value; }
        }

        private SBEEventEW _errorsBuild = new SBEEventEW();
        /// <summary>
        /// Errors during assembly
        /// </summary>
        public SBEEventEW errorsBuild
        {
            get { return _errorsBuild; }
            set { _errorsBuild = value; }
        }

        private SBEEventOWP _outputCustomBuild = new SBEEventOWP();
        /// <summary>
        /// Output-Build customization
        /// </summary>
        public SBEEventOWP outputCustomBuild
        {
            get { return _outputCustomBuild; }
            set { _outputCustomBuild = value; }
        }
    }
}
