/* eslint-disable prettier/prettier */
/* eslint-disable @next/next/no-sync-scripts */
import React from 'react';
import Head from 'next/head';
import { Placeholder, LayoutServiceData, Field, HTMLLink } from '@sitecore-jss/sitecore-jss-nextjs';
import config from 'temp/config';
// import Navigation from 'src/Navigation';
import Scripts from 'src/Scripts';
import { PageController } from '@sitecore-search/react';
// Prefix public assets with a public URL to enable compatibility with Sitecore editors.
// If you're not supporting Sitecore editors, you can remove this.
const publicUrl = config.publicUrl;

interface LayoutProps {
  layoutData: LayoutServiceData;
  headLinks: HTMLLink[];
}

interface RouteFields {
  [key: string]: unknown;
  pageTitle: Field;
}

const Layout = ({ layoutData, headLinks }: LayoutProps): JSX.Element => {
  const { route, context } = layoutData.sitecore;

  const fields = route?.fields as RouteFields;
  const lang = context?.language?.split('-') || ['en'];
  PageController.getContext().setLocaleLanguage(lang[0]);
  return (
    <>
      <Scripts />
      <Head>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
        <link
          rel="stylesheet"
          href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css"
        />
        <title>{fields.pageTitle.value.toString() || 'Page'}</title>
        <link rel="icon" href={`${publicUrl}/favicon.ico`} />
        {headLinks.map((headLink) => (
          <link rel={headLink.rel} key={headLink.href} href={headLink.href} />
        ))}
      </Head>

      {/* <Navigation /> */}
      {/* root placeholder for the app, which we add components to using route data */}
      {route && <Placeholder name="jss-header" rendering={route} />}
      <div className="container">{route && <Placeholder name="jss-main" rendering={route} />}</div>
    </>
  );
};

export default Layout;
