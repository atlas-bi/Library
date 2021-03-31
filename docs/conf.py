# flake8: noqa
# pylint: skip-file
# Configuration file for the Sphinx documentation builder.

#
# -- Project information -----------------------------------------------------
#

project = "Atlas of Information Management"
copyright = "2020, Riverside Healthcare"
author = "Riverside Healthcare"

#
# -- General configuration ---------------------------------------------------
#

extensions = [
    "sphinx.ext.extlinks",
    "sphinx.ext.doctest",
    "sphinx.ext.intersphinx",
    "sphinx.ext.viewcode",
    "sphinx.ext.todo",
    "sphinx_copybutton",
    "sphinx_inline_tabs",
    "sphinx_panels",
    "myst_parser",
    "sphinxcontrib.youtube",
]

templates_path = ["_templates"]

pygments_style = "colorful"

#
# -- Options for HTML output -------------------------------------------------
#

html_theme = "furo"
html_title = "Atlas Documentation"
html_logo = "images/icon.png"
html_favicon = "images/favicon.ico"

html_theme_options = {
    "sidebar_hide_name": True,
}

html_static_path = ["_static"]
