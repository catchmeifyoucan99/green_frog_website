/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	config.filebrowserBrowseUrl = "/Content/scripts/ckfinder/ckfinder.html";
	config.filebrowserImageUrl = "/Content/scripts/ckfinder/ckfinder.html?type=Images";
	config.filebrowserFlashUrl = "/Content/scripts/ckfinder/ckfinder.html?type=Flash";
	config.filebrowserUploadUrl = "/Content/scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files";
	config.filebrowserImageUploadUrl = "/Content/scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images";
	config.filebrowserFlashUploadUrl = "/Content/scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash";

	config.extraPlugins = 'youtube';
	config.youtube_responsive = true;
};
